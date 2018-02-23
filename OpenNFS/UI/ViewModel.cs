using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using OpenNFS.UI;

namespace OpenNFS
{
    public class ViewModel
    {
        public string WindowTitle
        {
            get; set;
        } = "OpenNFS | No file loaded";

        public ObservableCollection<Group> Groups { get; set; }

        public ViewModel()
        {
            Groups = new ObservableCollection<Group>();

            Groups.Add(new Group()
            {
                Key = 0,
                Name = "L5RA.BUN",
                SubGroups = new ObservableCollection<Group>()
                {
                    new Group()
                    {
                        Key = 0,
                        Name = "Sections",
                        IsEnabled = true,
                        SubGroups = new ObservableCollection<Group>()
                        {
                            new Group()
                            {
                                Key = 0,
                                Name = "T99",
                                IsEnabled = true,
                                SubGroups = new ObservableCollection<Group>()
                                {
                                    new Group()
                                    {
                                        Key = 0,
                                        Name = "Texture Packs",
                                        IsEnabled = true,
                                        Entries = new ObservableCollection<Entry>()
                                        {
                                            new TPKEntry()
                                            {
                                                Key = 0,
                                                Identifier = "TEMPTEXTURES",
                                                Path = "Location5\\TempTextures.tpk",
                                                IsEnabled = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Group()
                    {
                        Key = 1,
                        Name = "Something Else",
                        IsEnabled = true,
                    }
                },
                IsEnabled = true,
                IsSelected = true,
            });

            Groups.Add(new Group()
            {
                Key = 0,
                Name = "GLOBALB.BUN",
                SubGroups = new ObservableCollection<Group>()
                {
                    new Group()
                    {
                        Key = 0,
                        Name = "Cars",
                        IsEnabled = true,
                        Entries = new ObservableCollection<Entry>()
                        {
                            new CarDefinitionEntry()
                            {
                                Key = 0,
                                IDOne = "911TURBO",
                                IDTwo = "911TURBO",
                                ModelPath = "CARS\\911TURBO\\GEOMETRY.BIN",
                                Maker = "PORSCHE",
                                IsEnabled = true
                            },
                            new CarDefinitionEntry()
                            {
                                Key = 1,
                                IDOne = "CARRERAGT",
                                IDTwo = "CARRERAGT",
                                ModelPath = "CARS\\CARRERAGT\\GEOMETRY.BIN",
                                Maker = "PORSCHE",
                                IsEnabled = true
                            }
                        }
                    },
                    new Group()
                    {
                        Key = 1,
                        Name = "Materials",
                        IsEnabled = true,
                        Entries = new ObservableCollection<Entry>()
                        {
                            new Entry()
                            {
                                Key = 0,
                                Name = "ALUMINUM",
                                IsEnabled = true
                            },
                            new Entry()
                            {
                                Key = 1,
                                Name = "BOTTOM",
                                IsEnabled = true
                            },
                            new Entry()
                            {
                                Key = 2,
                                Name = "BRAKEDISC",
                                IsEnabled = true
                            },
                        }
                    }
                },
                IsEnabled = true,
            });
        }
    }
}
