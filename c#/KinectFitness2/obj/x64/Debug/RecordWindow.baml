   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   C    KinectFitness    KinectFitness.RecordWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   C   -   5          N� Title$  Record Window��5       N� WindowHeight$	 700Q�6�    N� WindowWidth$
 1300Q�6�    N� 
Background
 �   � N� ShowsNavigationUI .  .��5      �� -   5        $��
MainCanvas��5      .+�5	   
   ;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  -   5        $��kinectColorViewer1��5	   W    �� Left$ 0��6   
 �� Top$ 0��6/   $	��640��6>   $	��480��6J   5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 6q   ��  �� ElementName$ kinectSensorChooser1��	 �� Path$	 Kinect�=   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5
   
   -   5        $��kinectSensorChooser1��5
   <   $
��Left=�6!   $	��Top=�6X   $
��1300��6p   $	��549��6}   >   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  5   
   -   5        $��kinectSkeletonViewer1��5   E   $	 400��6"   $	 104��64   $	��320��6b   $	��240��6n    6{   �� $ kinectSensorChooser1��$	 Kinect��� 5   
   -   5        $��
stopButton��5   M   $	��150��6   $
��Left=�6   $��570,550,0,0q�68   $
��Fill=�6_   $	��Top=�6n   $	��150��6�   $3��-/KinectFitness;component/stopbuttonwindow.png��6�   �� 5   
   -   5        $��recordOption��5   ^   $��Record��6   $��23��6"   $
��Left=�6.   $��744,555,0,0q�6I   $	��Top=�6r   $��75��6�   6�   5%      5'      