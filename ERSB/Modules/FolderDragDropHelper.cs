﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace ERSB.Modules
{
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filepaths, string senderName);
    }

    public class FolderDragDropHelper
    {
        public static bool GetIsFileDragDropEnabled(DependencyObject obj) => (bool)obj.GetValue(IsFileDragDropEnabledProperty);

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value) => obj.SetValue(IsFileDragDropEnabledProperty, value);

        public static bool GetFileDragDropTarget(DependencyObject obj) => (bool)obj.GetValue(FileDragDropTargetProperty);

        public static void SetFileDragDropTarget(DependencyObject obj, bool value) => obj.SetValue(FileDragDropTargetProperty, value);

        public static readonly DependencyProperty IsFileDragDropEnabledProperty =
            DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FolderDragDropHelper),
                new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty =
            DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FolderDragDropHelper),
                null);

        private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            if (d is not Control control) return;
            control.Drop += OnDrop;
            control.DragOver += Control_DragOver;
        }

        private static void Control_DragOver(object sender, DragEventArgs e)
        {
            // e.Data.GetData(DataFormats.FileDrop);
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (sender is not DependencyObject d) return;
            var target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDragDropTarget fileTarget)
            {
                // if (_dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                //  {

                fileTarget.OnFileDrop((string[])dragEventArgs.Data.GetData(DataFormats.FileDrop), ((Control)sender).Name);

                //  }
            }
            else
            {
                throw new ArgumentException("FileDragDropTarget object must be of type IFileDragDropTarget");
            }
        }
    }
}
