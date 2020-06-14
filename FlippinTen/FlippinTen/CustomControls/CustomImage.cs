using Android.OS;
using FlippinTen.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.CustomControls
{
    public class CustomImage : Image
    {
        protected async override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(Source) || Source == null)
                return;

            Scale = 1.2;
            await this.ScaleTo(1, 250, Easing.Linear);
        }
    }
}
