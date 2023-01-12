﻿using ProniaAB101.Models;

namespace ProniaAB101.ViewModels
{
    public class UpdateProductImageVM
    {
        public IFormFile? CoverImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public IEnumerable<IFormFile>? OtherImages { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<int> ImageIds { get; set; }
    }
}
