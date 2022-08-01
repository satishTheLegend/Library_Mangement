using Acr.UserDialogs;
using Library_Mangement.Converters;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Services;
using Library_Mangement.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.Controls
{
    public class DynamicControlsView
    {
        #region Properties
        public readonly string strModuleName = "DynamicViews";
        #endregion

        #region Constructor
        public DynamicControlsView()
        {

        }
        #endregion

        public async Task LoadView(StackLayout ParentStack, ObservableCollection<DynamicPropertyDataViewModel> Items)
        {
            if(ParentStack == null)
            {
                ParentStack = new StackLayout();
            }
            View result = null;
            try
            {
                foreach (var DataItem in Items)
                {
                    
                    switch (DataItem.ControlType)
                    {
                        case "message":
                            result = MessageView(DataItem);
                            break;

                        case "TextBox":
                            result = TextInputView(DataItem);
                            break;

                        case "barcode1":
                            result = BarcodeView(DataItem);
                            break;

                        case "barcode2":
                            result = BarcodeView(DataItem);
                            break;

                        case "text-area":
                            result = TextAreaView(DataItem);
                            break;

                        case "radiobuttonlist":
                            result = await RadiobuttonlistView(DataItem);
                            break;

                        case "checkboxlist":
                            result = await CheckboxlistView(DataItem);
                            break;

                        case "dropdownlist":
                            result = await DropdownlistView(DataItem);
                            break;

                        case "DatePicker":
                            result = DatepickerView(DataItem);
                            break;

                        case "switch":
                            result = SwitchView(DataItem);
                            break;

                        default:
                            break;
                    }

                    if (result != null)
                        ParentStack.Children.Add(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Label MessageView(DynamicPropertyDataViewModel dataItem)
        {
            Label lbl = null;
            try
            {
                lbl = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, Text = dataItem.FieldName};
            }
            catch (Exception ex)
            {

            }
            return lbl;
        }

        private EntryControl TextInputView(DynamicPropertyDataViewModel dataItem)
        {
            EntryControl entryControl = null;
            try
            {
                entryControl = new EntryControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return entryControl;
        }

        private BarcodeControl BarcodeView(DynamicPropertyDataViewModel dataItem)
        {
            BarcodeControl barcodeControl = null;
            try
            {
                barcodeControl = new BarcodeControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return barcodeControl;
        }

        private TextAreaControl TextAreaView(DynamicPropertyDataViewModel dataItem)
        {
            TextAreaControl textAreaControl = null;
            try
            {
                textAreaControl = new TextAreaControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return textAreaControl;
        }
        private async Task<RadioButtonControl> RadiobuttonlistView(DynamicPropertyDataViewModel dataItem)
        {
            RadioButtonControl radioButtonGroup = null;
            try
            {
                //dataItem.OptValues = await GetOptValues(dataItem);
                radioButtonGroup = new RadioButtonControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return radioButtonGroup;
        }

        

        private async Task<CheckBoxListControl> CheckboxlistView(DynamicPropertyDataViewModel dataItem)
        {
            CheckBoxListControl checkBoxListControl = null;
            try
            {
                //dataItem.OptValues = await GetOptValues(dataItem);
                checkBoxListControl = new CheckBoxListControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return checkBoxListControl;
        }

        private async Task<DropDownListControl> DropdownlistView(DynamicPropertyDataViewModel dataItem)
        {
            DropDownListControl dropDownListControl = null;
            try
            {
                //dataItem.OptValues = await GetOptValues(dataItem);
                dropDownListControl = new DropDownListControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return dropDownListControl;
        }

        private View DatepickerView(DynamicPropertyDataViewModel dataItem)
        {
            DateTimePickerControl datePickerControl = null;
            try
            {
                datePickerControl = new DateTimePickerControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return datePickerControl;
        }

        private View SwitchView(DynamicPropertyDataViewModel dataItem)
        {
            SwitchControl switchControl = null;
            try
            {
                switchControl = new SwitchControl { ParentBindingContext = dataItem };
            }
            catch (Exception ex)
            {
                
            }
            return switchControl;
        }



        //private async Task<List<string>> GetOptValues(DynamicPropertyDataViewModel dataItem)
        //{
        //    List<string> list = null;
        //    if (dataItem.IncentiveDataModelUpdated == null)
        //    {
        //        var subRadioButtonList = await App.Database.ListObjectMaster.GetItemAsync(dataItem.DocTypeId);
        //        if (subRadioButtonList?.Count > 0)
        //        {
        //            list = subRadioButtonList.Select(y => y.value).ToList();
        //        }
        //    }
        //    else
        //    {
        //        var subRadioButtonUnitList = await App.Database.MeasureUnitValueMaster.GetItemAsync(dataItem.DocTypeId);
        //        if (subRadioButtonUnitList?.Count > 0)
        //        {
        //            list = subRadioButtonUnitList.Select(y => y.Values).ToList();
        //        }
        //    }
        //    return list;
        //}
    }
}
