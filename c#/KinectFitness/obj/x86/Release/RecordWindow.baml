   M S B A M L   `   `   `  ���� ^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers   KinectFitness    KinectFitness.RecordWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     -     � Title$  Record Window��
���   �.��� .+�=   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  -   $��kinectSensorChooser1��$
��Left=�$	��Top=�$
��1300��$	��549��>   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  -   $��kinectSkeletonViewer1��$��	0,0,0,151q�5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect��� -   $��recordButton��$	��150��$
��Left=�$��570,550,0,0q�$
��Fill=�$	��Top=�$	��150��$5��//KinectFitness;component/recordbuttonwindow.png�� K� ZIndex$ 1���� -   $��
stopButton��$	��150��$
��Left=�$��570,550,0,0q�$
��Fill=�$	��Top=�$	��150��$3��-/KinectFitness;component/stopbuttonwindow.png��� -    �Name$ 
SaveFileAs��$y�	Collapsed=�.+�� 
���   � J� Opacity$	 0.5Q��� $	��250��
��� ���
���   �$��1q� �� CornerRadius$ 	0,55,0,55��$��Center=�$��Center=�._��� .+��� $��5q�$��Save Recording As:��$
��Bold"�$��Cambria)��� -    ��Name$ InputTextBox��$	��150��$��Center=�$��Center=��� 	 �� Orientation$	 
Horizontal=�$��Center=�.+��� -   
 ��Name$
 	YesButton��$��5q�$
��Save����Z� �� -	   $
 NoButton��$��5q�$��Cancel����Z� 