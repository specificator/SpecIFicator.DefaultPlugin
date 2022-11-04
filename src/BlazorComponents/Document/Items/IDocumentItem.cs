﻿using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using SpecIFicator.Framework.CascadingValues;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.Document.Items
{
    public interface IDocumentItem
    {
        [CascadingParameter]
        public HierarchyContext DataContext { get; set; }

        string Type { get; }
    }
}
