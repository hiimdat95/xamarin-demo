using KamooniHost.IServices;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.PostsAndReviews
{
    public interface IUpdate
    {
        void OnUpdate(bool update);
    }

    internal class AddPlaceViewModel : TinyViewModel
    {
        private readonly IAddPlaceService addPlaceService;
        private readonly IUpdate updateResultListener;

        public AddPlaceViewModel(IAddPlaceService addPlaceService)
        {
            //LiveReload.Init();
            this.addPlaceService = addPlaceService;
        }

        public override void Init(object data)
        {
            //LiveReload.Init();
            base.Init(data);
        }
    }
}