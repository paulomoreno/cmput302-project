   M S B A M L   `   `   `  ���� ^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers   KinectFitness    KinectFitness.KinectWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     -     � Title$  Kinect Window��
���   � � SizeToContent$ WidthAndHeight=�.��� $	��700��$
��1500�� � ColumnDefinitions �� $
��575* ��� $
��725* �����  �� ImageSource$2 ,/KinectFitness;component/backgroundImage.jpg��.+��� $��	102,0,0,0q�$��2��.+�r� -   $��FitnessPlayer��$	��499��$
��Left=�$	��Top=�$	��750�� r� UnloadedBehavior$ Manual=� r� LoadedBehavior$ Manual=� K� ZIndex$ 1�� r� IsMuted .  $��2�� �� Left$	 100��
	 �� Top$	 90��
 J� OpacityMask
 Z� �� -   $��Doctor��$	 850��$	 0��$	��198��$
��Fill=�$	��260��$,��&/KinectFitness;component/no-avatar.jpg���� -   $��	rightHand��$	 204��$	 6��$��50��$
��Fill=�$��56��$/��)/KinectFitness;component/right%20hand.gif��$ 10���� -   $��btnUp��$
 1128��$	 32��$��	Camera Up��$��24��$��81�����	#FF1CB841
����������	#FF1CB841�� -   $��btnDown��$
 1128��$	 98��$��Camera Down��$��24��$��81�����	#FF1CB841
����������	#FF1CB841�� -   $��points��$	 856��$		 509��$��80��$��0 Pts.��$	��261��
�������$��64(�$��Corbel)��� -   $��playicon��$	 100��$		 591��$��84��$
��Fill=�$��82��$+��%/KinectFitness;component/playicon.png��$ 1���� -	   $��hoverplayicon��$	 100��$		 591��$��84��$
��Fill=�$��82��$0��*/KinectFitness;component/hoverplayicon.png�� J� Opacity$ 0Q��� -
   $��mediaBackground��$	 100��$	 0��$	��675��$	��750�����	#FF414141$ -3����Z� 
 Z� �� -   $��rectanglePlayer��$	 100��$		 591��$��84��$	��750�����	#FF7F7F7F$ -2����Z� �� -   $��	pauseicon��$	 100��$		 591��$��84��$
��Fill=�$��82��$,��&/KinectFitness;component/pauseicon.png��$ 0Q��� -   $��hoverpauseicon��$	 100��$		 591��$��84��$
��Fill=�$��82��$1��+/KinectFitness;component/hoverpauseicon.png��$ 0Q��� -   $��
heartgauge��$
 1904��$		 511��$	��150��$
��Fill=�$	��200��$5��//KinectFitness;component/heartpressuregauge.gif��$	 -10���� -   $��suggestionBlock��$	 278��$		 592��$��83��$�� ��$	��572��
�������$��60(�� -   $��	pointsBar��$	 848��$		 426��$��66��$	��262�� � Value$ 0Q��� -   $��videoProgressBar��$ 5��
�������$	 182��$		 591��$��84��$	��668����Z� �� -   $��rightHandProgressBar��$	 204��$	 6��$��0,50,0,0q�$	��Top=�$��52��$��12�����	#FF07FF1C$ 10����Z� �� -   $��userBackground�����	#FF414141$ -3��$��854,2,183,25q�$	 0.8Q�$ -4��$	 -2��$	��674��$	��268����Z� 
 Z� >   8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  -   $��kinectSkeletonViewer1��$	��263��$	��204��$	 850��$		 216��   	ShowBones .    
ShowCenter .    
ShowJoints . 5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect� � DataContext �� ��Z� �� -   $��backButtonHoverImg��$	��100��$
��Fill=�$	��100��$2��,/KinectFitness;component/backbuttonhover.gif��$ 0Q�$ 1��$	 12���� -   $��backButtonImg��$	��100��$
��Fill=�$	��100��$-��'/KinectFitness;component/backbutton.gif��$ 1��$	 12��=   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  -   $��kinectSensorChooser1��$	��256��$	��204��$	 856��$		 220���� -   $��debugger��$	 875��$		 672��$��0��$	��372��$
��asdf��$��48(��� -   $��bigPlayIconHoverImg��$	 378��$		 216��$	��204��$
��Fill=�$	��204��$0��*/KinectFitness;component/bigplaybutton.png��$ 0Q�$ 1���� -   $��bigPlayIconImg��$	 390��$		 228��$	��184��$
��Fill=�$	��184��$5��//KinectFitness;component/bigplaybuttonhover.png��$ 1���� -   $��statsBackground��$	��666��
���   �$
��1358��$ -5��$
 -106��$	 -2��
���   �$ 0Q��� -   $��angleStatsBox��$	 100��$		 106��$	��462��$	��368��$ -5��
����������	#FF217EE8$��40(�$ 0Q��� -   $��speedStatsBox��$	 500��$		 106��$	��538��$	��368��$ -5��
����������	#FF217EE8$��40(�$ 0Q��� -   $��
statsTitle��$	 356��$	 0��$	��110��$	��494��$ -5��
�������$��
Statistics��$��72(�$ 0Q���Z� ��Z� �� -   $��doneButtonImg��$	 910��$		 554��$��90��$
��Fill=�$	��200��$ -5��$-��'/KinectFitness;component/donebutton.gif��$ 0Q��� -    $��doneButtonHoverImg��$	 910��$		 554��$��90��$
��Fill=�$	��200��$2��,/KinectFitness;component/donebuttonhover.gif��$ -5��$ 0Q�