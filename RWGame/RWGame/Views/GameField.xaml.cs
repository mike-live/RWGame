using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp.Views.Forms;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameField : ContentPage
    {
        private GameFieldViewModel ViewModel {get; set;}
        public GameField(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ViewModel = new GameFieldViewModel(game, gameStateInfo, navigation);
            BindingContext = ViewModel;
        }
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            ViewModel.GameFieldDisplayData.PaintSurface(args);
        }
        private void OnCurCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            ViewModel.GameFieldDisplayData.GameControls.CurCanvasPaintSurface(args);
        }
    }
}
