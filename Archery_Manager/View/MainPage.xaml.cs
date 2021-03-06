﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Archery_Manager.View
{

    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>

    public sealed partial class MainPage : Bases.MvvmPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel.MainViewModel();
        }
    }
}
