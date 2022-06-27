﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradientColorControl : StackLayout
    {
        #region Constructor
        public GradientColorControl()
        {
            InitializeComponent();
            Task.Run(AnimateBackgorundColorAsync);
        }
        #endregion

        #region Private Methods
        private async Task AnimateBackgorundColorAsync()
        {
            {
                while (true)
                {
                    Action<double> forward = input => bdGradient.AnchorY = input;
                    Action<double> backward = input => bdGradient.AnchorY = input;
                    {
                        bdGradient.Animate(name: " forward ", callback: forward, start: 0, end: 1, length: 5000, easing: Easing.BounceIn);
                        await Task.Delay(5000);
                        bdGradient.Animate(name: " backward ", callback: backward, start: 1, end: 0, length: 5000, easing: Easing.SinIn);
                        await Task.Delay(5000);

                    }
                }
                #endregion
            }
        }
    }
}