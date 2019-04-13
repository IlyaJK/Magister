﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ConsumerBehavior.ViewModels
{
    class Function : BasicViewModel
    {
        public string Name { get; set; }
        public BitmapSource SourceFormula { get; set; }

        private Visibility _isVisibleFormula;

        public Visibility IsVisibleFormula
        {
            get { return _isVisibleFormula; }
            set
            {
                _isVisibleFormula = value;
                OnPropertyChanged(nameof(IsVisibleFormula));

            }
        }
    }
}
