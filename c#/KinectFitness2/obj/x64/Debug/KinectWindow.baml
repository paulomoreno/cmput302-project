   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   C    KinectFitness    KinectFitness.KinectWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   C   -   5          N� Title$  Kinect Window��5       N� WindowHeight$	 700Q�6�    N� WindowWidth$
 1300Q�6�    N� 
Background
 �   � N� ShowsNavigationUI .   � Triggers 5	      :� 5
   
    :� RoutedEvent$ Window.Loaded��6   .[�5      �� .��5      ��  g� AutoReverse .  g� 	BeginTime$ 0X�60   .��5      W� 	 �� 
TargetName$	 
background��6&   
 �� TargetProperty$
 Offset�5   &    W� From$	 0.2X�5   &   	 W� To$	 0.8X�61    g� Duration$ 0:0:10D�6:   5      5      5      .��5      �� ��5   
   �� 5   
    �� 
StartPoint$	 0,0&�6    �� EndPoint$	 1,1&�6/   .��5      �  � Color$ 	#FF3A76C4��6    � Offset$ 0Q�6-   � 5      $ 	#FF87A9F5��6   $	 0.2Q�6-   -   5        
background��5   ;   � 5      $ 	#FF3A76C4��6   $ 1Q�61   5      ._�5      � $	��700��6   $
��1300��6   .+�5   
   �� -   5        $��
MainCanvas��5      $��0q�6   .+�5      �� -   5        $��border1��5   O   
�������$��1q�6.   $	��240��6B   $	��320��6^    �� Left$	 979��6j   
 �� Top$	 210��6|   6�   >   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  5       -   5        $��kinectSkeletonViewer1��5    &   $	��240��6C   $	��320��6P   $	 977��6�   $	 210��6�   5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 6\   ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect�6�   �� 5!      -   5        $��Doctor��5!   C   $
 1032��6   $ 6��6'   $	��198��66   $
��Fill=�6Q   $	��262��6`   $,��&/KinectFitness;component/no-avatar.jpg��6l   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5"      -   5        $��kinectSensorChooser1��5"   %   $	��276��6A   $	��204��6M   $
 1005��6Z   $	 227��6m   �� 5#      -   5        $��	rightHand��5#   A   $	 104��6   $ 6��6&   $��50��65   $
��Fill=�6R   $��56��6a   $/��)/KinectFitness;component/right%20hand.gif��6l    K� ZIndex$ 3��6�   �� 5$      -	   5        $��leftHand��5$   @   $ 28��6   $ 6��6%   $��50��64   $
��Fill=�6P   $��54��6_   $.��(/KinectFitness;component/left%20hand.gif��6j   $ 3��6�   �� 5%      -
   5        $��btnUp��5%   X   $	 895��6   $	 319��6'   $��	Camera Up��68   $��24��6L   $��81��6{   ���	#FF1CB841
����������	#FF1CB8416�   �� 5&      -   5        $��btnDown��5&   Z   $	 895��6   $	 369��6'   $��Camera Down��68   $��24��6N   $��81��6�   ���	#FF1CB841
����������	#FF1CB8416�   �� 5'      -   5        $��points��5'   K   $	 977��6   $	 455��6.   $��80��6?   $��0 Pts.��6Y   $	��316��6g   
�������$��64(�6�   $��Corbel)�6�   �� 5(      -   5        $��playicon��5(   E   $ 0��6   $	 591��6(   $��84��69   $
��Fill=�6U   $��82��6d   $+��%/KinectFitness;component/playicon.png��6o   $ 1��6�   �� 5)      -   5        $��hoverplayicon��5)   E   $ 0��6   $	 591��6(   $��84��69   $
��Fill=�6Z   $��82��6i   $0��*/KinectFitness;component/hoverplayicon.png��6t    J� Opacity$ 0Q�6�   �� 5*      -   5        $��mediaBackground��5*   H   $ 0��6   $ 0��6,   $	��675��6;   $	��855��6q   ���	#FF414141$ -3��6�   ��6_   Z�  J� OpacityMask 6}   Z� 6�   �� 5+      -   5        $��rectanglePlayer��5+   I   $ 0��6   $	 591��6,   $��84��6=   $	��855��6r   ���	#FF7F7F7F$ -2��6�   ��6`   Z� 6�   �� 5,      -   5        $��	pauseicon��5,   E   $ 0��6   $	 591��6(   $��84��69   $
��Fill=�6V   $��82��6e   $,��&/KinectFitness;component/pauseicon.png��6p   $ 0Q�6�   �� 5-      -   5        $��hoverpauseicon��5-   E   $ 0��6   $	 591��6(   $��84��69   $
��Fill=�6[   $��82��6j   $1��+/KinectFitness;component/hoverpauseicon.png��6u   $ 0Q�6�   �� 5.      -   5        $��
heartgauge��5.   I   $
 1020��6   $	 538��6+   $	��150��6<   $
��Fill=�6[   $	��200��6j   $5��//KinectFitness;component/heartpressuregauge.gif��6v   �� 50      -   5        $��
heartarrow��51   D   $(��"/KinectFitness;component/arrow.gif��50      $
 1120��51      $	 629��6'   $��52��68   $
��Fill=�6V   $��49��6e   z�52      �� 53      -   5        AnimatedRotateTransform�� �� Angle$ 90Q�53   O   55      �� 56      -   5        $��suggestionBlock��56   K   $	 178��6   $	 592��6.   $��83��6?   $�� ��6b   $	��676��6j   
�������$��60(�6�   �� 57      -   5        $��kinectDisplay��57   D   $ 0��6   $ 1��6(   $	��591��67   $
��Fill=�6Y   $	��855��6h   ��6t   �� *Bitmap+�� 58      -   5        $��
viewButton��58   m   $	 869��6   $	 499��6+   $
��Play��6<   $��23��6K   $��75��6   �� 59      -   5        $��replayOption��59   Y   $	 869��6   $	 455��6+   $��Replay��6<   $��23��6M   $��75��6m   6x   �� 5:      -   5        $��
seatedMode��5:   `   $	 876��6   $	 188��6-   $��Seated Mode��6>   $��16��6T   6�   �� 5;      -   5        $��kinectDisplay2��5;   H   $	 765��6   $	 247��6*   $	��150��6;   $
��Fill=�6^   $	��200��6m   ��6y   �� *Bitmap+5<      5=      5>      5?      