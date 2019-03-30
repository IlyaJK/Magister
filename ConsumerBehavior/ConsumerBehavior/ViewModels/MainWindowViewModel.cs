using ConsumerBehavior;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
//using ConsumerBehavior.Models;
using ConsumerBehavior.Command;
using System.Collections.Generic;

namespace ConsumerBehavior.ViewModels
{
    class MainWindowViewModel : BasicViewModel
    {
        private string _caption = "Решение задачи оптимального поведения потребителя";

        public ObservableCollection<Function> Functions = new ObservableCollection<Function>();

        private Function _selectedFunction;

        public MainWindowViewModel()
        {
            var names = new string[3] { "Выберите функцию", "Аддитивная", "Логарифмическая" };
            foreach (var name in names)
            {
                Functions.Add(new Function() { Name = name });
            }
        }

        public Function SelectedFunction
        {
            get { return _selectedFunction; }
            set
            {
                _selectedFunction = value;
                OnPropertyChanged(nameof(SelectedFunction));
            }
        }

        private RelayCommand _onLoad;

        public RelayCommand OnLoad
        {
            get
            {
                return _onLoad ?? (_onLoad = new RelayCommand(param => 
                {
                   
                }));
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
    }



}