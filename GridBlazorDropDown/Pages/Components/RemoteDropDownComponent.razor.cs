using GridBlazorDropDown.Data;
using GridBlazor;
using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorDropDown.Pages.Components
{
    public partial class RemoteDropDownComponent<T, V> : ICustomGridComponent<T> where V : class//, IGenericModel
    {
        [Parameter]
        public T Item { get; set; }

        [Parameter]
        public CGrid<T> Grid { get; set; }

        [Parameter]
        public object Object { get; set; }

        [Inject]
        private IGenericService<V> IGenericService { get; set; }

        public IEnumerable<SelectItem> SelectedItems;
        public string selectedValue;
        private string searchTerm;
        public bool allowChange;
        private string _foreignKeyField;
        private string _message;
        private Func<T, string?> _expr;
        private ModelExtension Model { get; set; }

        public string SearchTerm
        {
            get { return searchTerm; }
            set { searchTerm = value; OnSearchChange(); }
        }

        protected override void OnParametersSet()
        {
            if (Object.GetType() == typeof((string, Func<T, string?>)))
            {
                (_foreignKeyField, _expr) = ((string, Func<T, string?>))Object;
                try
                {
                    Model = new ModelExtension(typeof(T), Item);
                    selectedValue = Model.GetValue($"{_foreignKeyField}")?.ToString() ?? "";
                    SearchTerm = _expr(Item) ?? "";
                }
                catch (Exception e)
                {
                    throw new Exception("ERROR RemoteDropDownComponent must have (string[], Func<T, string?>) parameters and GetSelectedItems");
                }
            }

            string gridState = Grid.GetState();
            allowChange = Grid.Mode == GridMode.Update || Grid.Mode == GridMode.Create;
            if (!allowChange)
            {
                selectedValue = String.Concat(Model.GetValue($"{_foreignKeyField}")?.ToString(), " - ", _expr(Item));
            }
        }

        public void OnSearchChange()
        {
            SelectedItems = IGenericService.GetSelect(searchTerm);
            _message = "-";// $"to select... [{SelectedItems.Count()}]";
        }

        private void ChangeValue(ChangeEventArgs e)
        {
            var value = e?.Value?.ToString();
            if (value != "")
            {
                Model.SetValue($"{_foreignKeyField}", value);
            }
        }
    }
}