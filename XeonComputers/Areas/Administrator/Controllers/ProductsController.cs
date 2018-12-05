﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XeonComputers.Services.Contracts;
using XeonComputers.Areas.Administrator.ViewModels.Products;
using XeonComputers.Common;
using XeonComputers.Data;
using XeonComputers.Models;

namespace XeonComputers.Areas.Administrator.Controllers
{
    public class ProductsController : AdministratorController
    {
        private IProductsService productService;
        private IImagesService imageService;

        public ProductsController(IProductsService productService, IImagesService imageService)
        {
            this.productService = productService;
            this.imageService = imageService;
        }

        public IActionResult All()
        {
            var products = this.productService.GetProducts();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = this.productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = new DetailsProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ChildCategoryName = product.ChildCategory.Name,
                Price = product.Price,
                ParnersPrice = product.ParnersPrice,
                ProductType = product.ProductType,
                Specification = product.Specification
            };

            return View(productViewModel);
        }

        public IActionResult Create()
        {
            var childCategories = this.productService.GetChildCategories();

            var categories = childCategories.Select(x => new SelectListItem
                                                        {
                                                           Value = x.Id.ToString(),
                                                           Text = x.Name
                                                        }).ToList();

            var model = new CreateProductViewModel { ChildCategories = categories };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var childCategories = this.productService.GetChildCategories();

                ViewData["ChildCategoryId"] = new SelectList(childCategories, "Id", "Id", model.ChildCategoryId);

                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                ParnersPrice = model.ParnersPrice,
                Price = model.Price,
                Description = model.Description,
                Specification = model.Specification,
                ChildCategoryId = model.ChildCategoryId
            };

            this.productService.AddProduct(product);

            if (model.FormImages != null)
            {
                int existingImages = 0;
                var imageUrls = await this.imageService.UploadImages(model.FormImages.ToList(), existingImages,
                                                                GlobalConstans.PRODUCT_PATH_TEMPLATE, product.Id);

                this.productService.AddImageUrls(product.Id, imageUrls);
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult Edit(int id)
        {
            var product = this.productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var childCategories = this.productService.GetChildCategories();

            ViewData["ChildCategoryId"] = new SelectList(childCategories, "Id", "Id", product.ChildCategoryId);

            var model = new EditProductViewModel
            {
                Name = product.Name,
                ParnersPrice = product.ParnersPrice,
                Price = product.Price,
                Description = product.Description,
                Specification = product.Specification,
                ChildCategoryId = product.ChildCategoryId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var childCategories = this.productService.GetChildCategories();

                ViewData["ChildCategoryId"] = new SelectList(childCategories, "Id", "Id", model.ChildCategoryId);

                return View(model);
            }

            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                ParnersPrice = model.ParnersPrice,
                Price = model.Price,
                Description = model.Description,
                Specification = model.Specification,
                ChildCategoryId = model.ChildCategoryId
            };

            this.productService.EditProduct(product);

            if (model.FormImages != null)
            {
                int existingImages = productService.GetImages(product.Id).Count();
                var imageUrls = await this.imageService.UploadImages(model.FormImages.ToList(), existingImages,
                                                                GlobalConstans.PRODUCT_PATH_TEMPLATE, product.Id);

                this.productService.AddImageUrls(product.Id, imageUrls);
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult Delete(int id)
        {
            var product = this.productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            this.productService.RemoveProduct(id);

            return RedirectToAction(nameof(All));
        }
    }
}