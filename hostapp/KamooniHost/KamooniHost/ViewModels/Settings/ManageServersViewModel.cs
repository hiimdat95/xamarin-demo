using KamooniHost.Models;
using SQLite;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class ManageServersViewModel : TinyViewModel
    {
        private readonly SQLiteConnection localDb;

        public Host CurrentHost => Helpers.Settings.CurrentHost;

        public ObservableCollection<Host> Hosts { get; set; } = new ObservableCollection<Host>();

        public ICommand AddServerCommand { get; private set; }
        public ICommand SelectHostCommand { get; private set; }
        public ICommand DeleteHostCommand { get; private set; }

        public ManageServersViewModel(SQLiteConnection localDb)
        {
            this.localDb = localDb;

            AddServerCommand = new AwaitCommand(AddServer);
            SelectHostCommand = new AwaitCommand<Host>(SelectHost);
            DeleteHostCommand = new AwaitCommand<Host>(DeleteHost);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetListHost();
        }

        private void GetListHost()
        {
            Hosts.Clear();
            Hosts.AddRange(localDb.Table<Host>().ToList().FindAll(h => h.Name != CurrentHost.Name));
        }

        private void SelectHost(Host host, TaskCompletionSource<bool> tcs)
        {
            Helpers.Settings.HostID = host.Id;

            Helpers.Settings.CurrentHost = host;

            Application.Current.MainPage = new MainPage();
            tcs.SetResult(true);
        }

        private void DeleteHost(Host host, TaskCompletionSource<bool> tcs)
        {
            Hosts.Remove(host);
            localDb.Delete(host);
            tcs.SetResult(true);
        }

        private async void AddServer(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<LoginViewModel>();
            tcs.SetResult(true);
        }
    }
}