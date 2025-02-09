﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using ColorPicker.Common;
using ColorPicker.Helpers;
using ColorPicker.Models;
using ColorPicker.Settings;
using ColorPicker.ViewModelContracts;
using Microsoft.PowerToys.Settings.UI.Library.Enumerations;
using Microsoft.Win32;

namespace ColorPicker.ViewModels
{
    [Export(typeof(IColorEditorViewModel))]
    public class ColorEditorViewModel : ViewModelBase, IColorEditorViewModel
    {
        private readonly IUserSettings _userSettings;
        private readonly List<ColorFormatModel> _allColorRepresentations = new List<ColorFormatModel>();
        private Color _selectedColor;
        private bool _initializing;
        private int _selectedColorIndex;
        private int _selectedPinnedColorIndex;

        [ImportingConstructor]
        public ColorEditorViewModel(IUserSettings userSettings)
        {
            OpenColorPickerCommand = new RelayCommand(() => OpenColorPickerRequested?.Invoke(this, EventArgs.Empty));
            OpenSettingsCommand = new RelayCommand(() => OpenSettingsRequested?.Invoke(this, EventArgs.Empty));

            RemoveColorsCommand = new RelayCommand(DeleteSelectedColors);
            RemovePinnedColorsCommand = new RelayCommand(DeleteSelectedPinnedColors);
            ClearAllPinnedColorsCommand = new RelayCommand(RemoveAllPinnedColors);
            PinColorsCommand = new RelayCommand(PinSelectedColors);
            ExportColorsGroupedByColorCommand = new RelayCommand(ExportSelectedColorsByColor);
            ExportColorsGroupedByFormatCommand = new RelayCommand(ExportSelectedColorsByFormat);
            SelectedColorChangedCommand = new RelayCommand((newColor) =>
            {
                if (ColorsHistory.Contains((Color)newColor))
                {
                    ColorsHistory.Remove((Color)newColor);
                }

                ColorsHistory.Insert(0, (Color)newColor);
                SelectedColorIndex = 0;
            });
            ColorsHistory.CollectionChanged += ColorsHistory_CollectionChanged;
            PinnedColors.CollectionChanged += PinnedColors_CollectionChanged;
            _userSettings = userSettings;
            SetupAllColorRepresentations();
            SetupAvailableColorRepresentations();
        }

        public event EventHandler OpenColorPickerRequested;

        public event EventHandler OpenSettingsRequested;

        public ICommand OpenColorPickerCommand { get; }

        public ICommand OpenSettingsCommand { get; }

        public ICommand RemoveColorsCommand { get; }

        public ICommand RemovePinnedColorsCommand { get; }

        public ICommand PinColorsCommand { get; }

        public ICommand ClearAllPinnedColorsCommand { get; }

        public ICommand ExportColorsGroupedByColorCommand { get; }

        public ICommand ExportColorsGroupedByFormatCommand { get; }

        public ICommand SelectedColorChangedCommand { get; }

        public ICommand HideColorFormatCommand { get; }

        public ObservableCollection<Color> ColorsHistory { get; } = new ObservableCollection<Color>();

        public ObservableCollection<Color> PinnedColors { get; } = new ObservableCollection<Color>();

        public int ColorCount => ColorsHistory.Count + PinnedColors.Count;

        public ObservableCollection<ColorFormatModel> ColorRepresentations { get; } = new ObservableCollection<ColorFormatModel>();

        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }

            set
            {
                _selectedColor = value;
                OnPropertyChanged();
            }
        }

        public int SelectedColorIndex
        {
            get
            {
                return _selectedColorIndex;
            }

            set
            {
                _selectedColorIndex = value;
                if (value >= 0)
                {
                    SelectedColor = ColorsHistory[_selectedColorIndex];
                    if (ColorsHistory.Count > 0 && PinnedColors.Count > 0)
                    {
                        SelectedPinnedColorIndex = -1;
                    }
                }
                else
                {
                    if (ColorsHistory.Count == 0 && PinnedColors.Count > 0)
                    {
                        SelectedPinnedColorIndex = 0;
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedPinnedColorIndex));
            }
        }

        public int SelectedPinnedColorIndex
        {
            get
            {
                return _selectedPinnedColorIndex;
            }

            set
            {
                _selectedPinnedColorIndex = value;
                if (value >= 0)
                {
                    SelectedColor = PinnedColors[_selectedPinnedColorIndex];
                    if (ColorsHistory.Count > 0 && PinnedColors.Count > 0)
                    {
                        SelectedColorIndex = -1;
                    }
                }
                else
                {
                    if (PinnedColors.Count == 0 && ColorsHistory.Count > 0 )
                    {
                        SelectedColorIndex = 0;
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedColorIndex));
            }
        }

        public void Initialize()
        {
            _initializing = true;

            ColorsHistory.Clear();
            PinnedColors.Clear();

            foreach (var item in _userSettings.ColorHistory)
            {
                ColorsHistory.Add(ParseStringToColor(item));
                SelectedColorIndex = 0;
            }

            foreach (var item in _userSettings.PinnedColors)
            {
                PinnedColors.Add(ParseStringToColor(item));
                SelectedColorIndex = 0;
            }

            _initializing = false;

            static Color ParseStringToColor(string color)
            {
                var parts = color.Split('|');
                return new Color()
                {
                    A = byte.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture),
                    R = byte.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture),
                    G = byte.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture),
                    B = byte.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture),
                };
            }
        }

        private void ColorsHistory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_initializing)
            {
                _userSettings.ColorHistory.ClearWithoutNotification();
                foreach (var item in ColorsHistory)
                {
                    _userSettings.ColorHistory.AddWithoutNotification(item.A + "|" + item.R + "|" + item.G + "|" + item.B);
                }

                _userSettings.ColorHistory.ReleaseNotification();
            }

            OnPropertyChanged(nameof(ColorCount));
        }

        private void PinnedColors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_initializing)
             {
                _userSettings.PinnedColors.ClearWithoutNotification();
                foreach (var item in PinnedColors)
                {
                    _userSettings.PinnedColors.AddWithoutNotification(item.A + "|" + item.R + "|" + item.G + "|" + item.B);
                }

                _userSettings.PinnedColors.ReleaseNotification();
           }

            OnPropertyChanged(nameof(ColorCount));
        }

        private void DeleteSelectedColors(object selectedColors)
        {
            var colorsToRemove = ((IList)selectedColors).OfType<Color>().ToList();
            var indicesToRemove = colorsToRemove.Select(color => ColorsHistory.IndexOf(color)).ToList();

            foreach (var color in colorsToRemove)
            {
                ColorsHistory.Remove(color);
            }

            SelectedColorIndex = ComputeWhichIndexToSelectAfterDeletion(colorsToRemove.Count + ColorsHistory.Count, indicesToRemove);
            SessionEventHelper.Event.EditorHistoryColorRemoved = true;
        }

        private void DeleteSelectedPinnedColors(object selectedColors)
        {
            var colorsToRemove = ((IList)selectedColors).OfType<Color>().ToList();
            var indicesToRemove = colorsToRemove.Select(color => PinnedColors.IndexOf(color)).ToList();

            foreach (var color in colorsToRemove)
            {
                PinnedColors.Remove(color);
            }

            SelectedPinnedColorIndex = ComputeWhichIndexToSelectAfterDeletion(colorsToRemove.Count + PinnedColors.Count, indicesToRemove);
            SessionEventHelper.Event.EditorHistoryColorRemoved = true;
        }

        private void RemoveAllPinnedColors()
        {
            PinnedColors.Clear();
            _userSettings.PinnedColors.Clear();
        }

        private void PinSelectedColors(object selectedColors)
        {
            var colorsToPin = ((IList)selectedColors).OfType<Color>().ToList();

            foreach (var color in colorsToPin)
            {
                if (PinnedColors.Contains(color))
                {
                    PinnedColors.Remove(color);
                }

                PinnedColors.Insert(0, color);
            }

            foreach (var color in colorsToPin)
            {
                if (_userSettings.PinnedColors.Count > _userSettings.ColorHistoryLimit.Value)
                {
                    _userSettings.PinnedColors.RemoveAt(_userSettings.PinnedColors.Count - 1);
                }
            }

            SessionEventHelper.Event.EditorHistoryColorPinned = true;
        }

        private void ExportSelectedColorsByColor(object selectedColors)
        {
            ExportColors(selectedColors, GroupExportedColorsBy.Color);
        }

        private void ExportSelectedColorsByFormat(object selectedColors)
        {
            ExportColors(selectedColors, GroupExportedColorsBy.Format);
        }

        private void ExportColors(object colorsToExport, GroupExportedColorsBy method)
        {
            var colors = SerializationHelper.ConvertToDesiredColorFormats((IList)colorsToExport, ColorRepresentations, method);

            var dialog = new SaveFileDialog
            {
                Title = "Save selected colors to",
                Filter = "Text Files (*.txt)|*.txt|Json Files (*.json)|*.json",
            };

            if (dialog.ShowDialog() == true)
            {
                var extension = Path.GetExtension(dialog.FileName);

                var contentToWrite = extension.ToUpperInvariant() switch
                {
                    ".TXT" => colors.ToTxt(';'),
                    ".JSON" => colors.ToJson(),
                    _ => string.Empty,
                };

                File.WriteAllText(dialog.FileName, contentToWrite);
                SessionEventHelper.Event.EditorColorsExported = true;
            }
        }

        // Will select the closest color to the last selected one in color history
        private static int ComputeWhichIndexToSelectAfterDeletion(int colorsCount, List<int> indicesToRemove)
        {
            var newIndices = Enumerable.Range(0, colorsCount).ToList();
            foreach (var index in indicesToRemove)
            {
                newIndices[index] = -1;
            }

            var appearancesOfMinusOne = 0;
            for (var i = 0; i < newIndices.Count; i++)
            {
                if (newIndices[i] < 0)
                {
                    appearancesOfMinusOne++;
                    continue;
                }

                newIndices[i] -= appearancesOfMinusOne;
            }

            var lastSelectedIndex = indicesToRemove.Last();
            for (int i = lastSelectedIndex - 1, j = lastSelectedIndex + 1; ; i--, j++)
            {
                if (j < newIndices.Count && newIndices[j] != -1)
                {
                    return newIndices[j];
                }

                if (i >= 0 && newIndices[i] != -1)
                {
                    return newIndices[i];
                }

                if (j >= newIndices.Count && i < 0)
                {
                    break;
                }
            }

            return -1;
        }

        private void SetupAllColorRepresentations()
        {
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HEX.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HEX.ToString()).ToLowerInvariant(),
                });

            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.RGB.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.RGB.ToString()),
                });

            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HSL.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HSL.ToString()),
                });

            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HSV.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HSV.ToString()),
                });

            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.CMYK.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.CMYK.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HSB.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HSB.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HSI.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HSI.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.HWB.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.HWB.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.NCol.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.NCol.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.CIELAB.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.CIELAB.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.CIEXYZ.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.CIEXYZ.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = ColorRepresentationType.VEC4.ToString(),
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, ColorRepresentationType.VEC4.ToString()),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = "Decimal",
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, "Decimal"),
                });
            _allColorRepresentations.Add(
                new ColorFormatModel()
                {
                    FormatName = "HEX Int",
                    Convert = (Color color) => ColorRepresentationHelper.GetStringRepresentationFromMediaColor(color, "HEX Int"),
                });

            _userSettings.VisibleColorFormats.CollectionChanged += VisibleColorFormats_CollectionChanged;

            // Any other custom format to be added here as well that are read from settings
        }

        private void VisibleColorFormats_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetupAvailableColorRepresentations();
        }

        private void SetupAvailableColorRepresentations()
        {
            ColorRepresentations.Clear();

            foreach (var colorFormat in _userSettings.VisibleColorFormats)
            {
                ColorRepresentations.Add(new ColorFormatModel() { FormatName = colorFormat.Key.ToUpperInvariant(), Convert = null, FormatString = colorFormat.Value });
            }
        }
    }
}
