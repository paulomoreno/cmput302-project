   M S B A M L   `   `   `  ���� ^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers   KinectFitness    KinectFitness.KinectWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     -     N� Title$  Kinect Window�� N� WindowHeight$	 700Q� N� WindowWidth$
 1300Q� N� 
Background
 �   � N� ShowsNavigationUI .   � Triggers :�  :� RoutedEvent$ Window.Loaded��.[��� .����  g� AutoReverse .  g� 	BeginTime$ 0X�.��W� 	 �� 
TargetName$	 
background��
 �� TargetProperty$
 Offset� W� From$	 0.2X�	 W� To$	 0.8X� g� Duration$ 0:0:10D�.���� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ 	#FF3A76C4�� � Offset$ 0Q�� $ 	#FF87A9F5��$	 0.2Q�-   
background��� $ 	#FF3A76C4��$ 1Q�._�� $	��700��$
��1300��.+��� -   $��
MainCanvas��$��0q�.+��� -   $��border1��
�������$��1q�$	��240��$	��320�� �� Left$	 979��
 �� Top$	 210��>   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  -   $��kinectSkeletonViewer1��$	��240��$	��320��$	 977��$	 210��5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect��� -   $��Doctor��$
 1032��$ 6��$	��198��$
��Fill=�$	��262��$,��&/KinectFitness;component/no-avatar.jpg��=   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  -   $��kinectSensorChooser1��$	��276��$	��204��$
 1005��$	 227���� -   $��	rightHand��$	 104��$ 6��$��50��$
��Fill=�$��56��$/��)/KinectFitness;component/right%20hand.gif�� K� ZIndex$ 3���� -	   $��leftHand��$ 28��$ 6��$��50��$
��Fill=�$��54��$.��(/KinectFitness;component/left%20hand.gif��$ 3���� -
   $��btnUp��$	 895��$	 319��$��	Camera Up��$��24��$��81�����	#FF1CB841
����������	#FF1CB841�� -   $��btnDown��$	 895��$	 369��$��Camera Down��$��24��$��81�����	#FF1CB841
����������	#FF1CB841�� -   $��points��$	 977��$	 455��$��80��$��0 Pts.��$	��316��
�������$��64(�$��Corbel)��� -   $��playicon��$ 0��$	 591��$��84��$
��Fill=�$��82��$+��%/KinectFitness;component/playicon.png��$ 1���� -   $��hoverplayicon��$ 0��$	 591��$��84��$
��Fill=�$��82��$0��*/KinectFitness;component/hoverplayicon.png�� J� Opacity$ 0Q��� -   $��mediaBackground��$ 0��$ 0��$	��675��$	��855�����	#FF414141$ -3����Z�  J� OpacityMask Z� �� -   $��rectanglePlayer��$ 0��$	 591��$��84��$	��855�����	#FF7F7F7F$ -2����Z� �� -   $��	pauseicon��$ 0��$	 591��$��84��$
��Fill=�$��82��$,��&/KinectFitness;component/pauseicon.png��$ 0Q��� -   $��hoverpauseicon��$ 0��$	 591��$��84��$
��Fill=�$��82��$1��+/KinectFitness;component/hoverpauseicon.png��$ 0Q��� -   $��
heartgauge��$
 1020��$	 538��$	��150��$
��Fill=�$	��200��$5��//KinectFitness;component/heartpressuregauge.gif���� -   $��
heartarrow��$(��"/KinectFitness;component/arrow.gif��$
 1120��$	 629��$��52��$
��Fill=�$��49��z��� -   AnimatedRotateTransform�� �� Angle$ 90Q��� -   $��suggestionBlock��$	 178��$	 592��$��83��$�� ��$	��676��
�������$��60(��� -   $��kinectDisplay��$ 0��$ 1��$	��591��$
��Fill=�$	��855������ *Bitmap+�� -   $��
viewButton��$	 869��$	 499��$
��Play��$��23��$��75���� -   $��replayOption��$	 869��$	 455��$��Replay��$��23��$��75���� -   $��
seatedMode��$	 876��$	 188��$��Seated Mode��$��16���� -   $��kinectDisplay2��$	 765��$	 247��$	��150��$
��Fill=�$	��200������ *Bitmap+