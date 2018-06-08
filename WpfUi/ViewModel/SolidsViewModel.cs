using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using LibOpenNFS.DataModels;
using WpfUi.Services;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    /// <summary>
    /// A solid proxy object used in the view model.
    /// </summary>
    public class SolidProxy : ObservableObject
    {
        private uint _hash;
        private string _name;

        public uint Hash
        {
            get => _hash;
            set
            {
                _hash = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// The view model for a solid list display.
    /// </summary>
    public class SolidsViewModel : DockWindowViewModel
    {
        private readonly IResourceService _resourceService;
        private readonly string _listId;
        private readonly string _sectionId;

        private string _groupId;

        public string GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                RaisePropertyChanged();
            }
        }

        public SolidList SolidList => _resourceService.FindSolidList($"{_listId}_{_sectionId}", _groupId);

        public ObservableCollection<SolidProxy> Solids { get; }

        public RelayCommand<SolidProxy> ViewSolidCommand { get; }

        public SolidsViewModel(SolidListResource solidList)
        {
            _resourceService = SimpleIoc.Default.GetInstance<IResourceService>();
            _groupId = solidList.GroupId;
            _sectionId = solidList.SectionId;
            _listId = solidList.ListName;

            Solids = new ObservableCollection<SolidProxy>(SolidList.Objects.Select(obj => new SolidProxy
            {
                Name = obj.Name,
                Hash = obj.Hash
            }));

            ViewSolidCommand = new RelayCommand<SolidProxy>(Console.WriteLine);
            Title = $"Solid List - {SolidList.SectionId}";
        }
    }
}
