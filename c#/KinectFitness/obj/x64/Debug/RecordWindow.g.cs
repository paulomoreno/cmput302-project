﻿#pragma checksum "..\..\..\RecordWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "78118C87810A19486CD5E1768C3D2EB7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Samples.Kinect.WpfViewers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace KinectFitness {
    
    
    /// <summary>
    /// RecordWindow
    /// </summary>
    public partial class RecordWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser kinectSensorChooser1;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer kinectSkeletonViewer1;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image recordButton;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image stopButton;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SaveFileAs;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputTextBox;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button YesButton;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\RecordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NoButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/KinectFitness;component/recordwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\RecordWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 6 "..\..\..\RecordWindow.xaml"
            ((KinectFitness.RecordWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            
            #line 6 "..\..\..\RecordWindow.xaml"
            ((KinectFitness.RecordWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.kinectSensorChooser1 = ((Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser)(target));
            return;
            case 3:
            this.kinectSkeletonViewer1 = ((Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer)(target));
            
            #line 10 "..\..\..\RecordWindow.xaml"
            this.kinectSkeletonViewer1.Unloaded += new System.Windows.RoutedEventHandler(this.kinectSkeletonViewer1_Unloaded);
            
            #line default
            #line hidden
            return;
            case 4:
            this.recordButton = ((System.Windows.Controls.Image)(target));
            
            #line 11 "..\..\..\RecordWindow.xaml"
            this.recordButton.AddHandler(System.Windows.Input.Mouse.MouseUpEvent, new System.Windows.Input.MouseButtonEventHandler(this.Record_Button));
            
            #line default
            #line hidden
            return;
            case 5:
            this.stopButton = ((System.Windows.Controls.Image)(target));
            return;
            case 6:
            this.SaveFileAs = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.InputTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.YesButton = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\RecordWindow.xaml"
            this.YesButton.Click += new System.Windows.RoutedEventHandler(this.SaveButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.NoButton = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\..\RecordWindow.xaml"
            this.NoButton.Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

