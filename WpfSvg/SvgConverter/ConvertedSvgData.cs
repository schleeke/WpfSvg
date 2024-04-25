﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSvg.SvgConverter {
    internal class ConvertedSvgData {

        private string _filepath;
        private string _xaml;
        private string _svg;
        private string _objectName;
        private DependencyObject _convertedObj;

        public string Filepath {
            get { return _filepath; }
            set { _filepath = value; }
        }

        public string Xaml {
            get { return _xaml ?? (_xaml = ConverterLogic.SvgObjectToXaml(ConvertedObj, false, _objectName, false)); }
            set { _xaml = value; }
        }

        public string Svg {
            get { return _svg ?? (_svg = System.IO.File.ReadAllText(_filepath)); }
            set { _svg = value; }
        }

        public DependencyObject ConvertedObj {
            get {
                if (_convertedObj == null) {
                    _convertedObj = ConverterLogic.ConvertSvgToObject(_filepath, ResultMode.DrawingImage, null, out _objectName, new ResKeyInfo()) as DependencyObject;
                }
                return _convertedObj;
            }
            set { _convertedObj = value; }
        }

    }
}
