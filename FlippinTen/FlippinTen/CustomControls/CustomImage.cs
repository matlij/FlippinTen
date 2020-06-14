using System.Runtime.CompilerServices;
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

            await this.ScaleTo(0.8, 125, Easing.Linear);
            await this.ScaleTo(1, 125, Easing.Linear);
        }
    }
}
