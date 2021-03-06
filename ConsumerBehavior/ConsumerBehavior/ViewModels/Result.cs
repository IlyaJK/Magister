﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ConsumerBehavior.ViewModels
{
    class Result : BasicViewModel
    {
        public BitmapSource ItemResult { get; set; }
        public HorizontalAlignment Align { get; set; }
        public Visibility TopPadding { get; set; }
    }
}
