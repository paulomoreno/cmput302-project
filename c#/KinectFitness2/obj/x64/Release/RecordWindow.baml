   M S B A M L   `   `   `  ���� ^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers   KinectFitness    KinectFitness.RecordWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     -     N� Title$  Record Window�� N� WindowHeight$	 700Q� N� WindowWidth$
 1300Q� N� 
Background
 �   � N� ShowsNavigationUI .  .���� -   $��
MainCanvas��.+�;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  -   $��kinectColorViewer1�� �� Left$ 0��
 �� Top$ 0��$	��640��$	��480��5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect ��  �� ElementName$ kinectSensorChooser1��	 �� Path$	 Kinect�=   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  -   $��kinectSensorChooser1��$
��Left=�$	��Top=�$
��1300��$	��549��>   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  -   $��kinectSkeletonViewer1��$	 400��$	 104��$	��320��$	��240�� �� $ kinectSensorChooser1��$	 Kinect��� -   $��
stopButton��$	��150��$
��Left=�$��570,550,0,0q�$
��Fill=�$	��Top=�$	��150��$3��-/KinectFitness;component/stopbuttonwindow.png���� -   $��recordOption��$��Record��$��23��$
��Left=�$��744,555,0,0q�$	��Top=�$��75��