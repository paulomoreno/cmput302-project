   M S B A M L   `   `   `  ����   KinectFitness!    KinectFitness.StartupWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008       N� Title$  Kinect Fitness�� N� WindowHeight$	 800Q� N� WindowWidth$
 1000Q�$	��800��$
��1000�� � Triggers :�  :� RoutedEvent$ Window.Loaded��.[��� .����  g� AutoReverse .  g� 	BeginTime$ 0X�.��W�  �� 
TargetName$ 
background�� �� TargetProperty$ Offset�	 W� From$		 0.2X�	
 W� To$	
 0.8X� g� Duration$ 0:0:10D�.���� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ 	#FF454545�� � Offset$ 0Q�� $ 	#FF9D9D9D��$	 0.2Q�-   
background��� $ 	#FF313131��$ 1Q�._�� $��
10,0,10,10q���Z� .+��� -   $��
playButton��$��1��$	��164��$
��Left=�$��400,250,0,0q�$
��Fill=�$	��Top=�$	��172��$-��'/KinectFitness;component/playbutton.png���� -   $��optionsButton��$	��164��$
��Left=�$��100,250,0,0q�$
��Fill=�$	��Top=�$	��172��$0��*/KinectFitness;component/optionsbutton.png��$��1���� -   $��tutorialbutton��$��1��$	��164��$
��Left=�$��700,250,0,0q�$
��Fill=�$	��Top=�$	��172��$1��+/KinectFitness;component/tutorialbutton.png���� -   $��
quitbutton��$��1��$	��164��$
��Left=�$��	400,0,0,0q�$
��Fill=�$	��Top=�$	��172��$-��'/KinectFitness;component/quitbutton.png���� -   $��recordbutton��$��1��$	��164��$
��Left=�$��400,500,0,0q�$
��Fill=�$	��Top=�$	��172��$/��)/KinectFitness;component/recordbutton.png���� -   $��
playborder��
�������$��5q�$��1��$	��164��$
��Left=�$��400,250,0,0q�$	��Top=�$	��172�� J� OpacityMask
 ����� J� Opacity$ 0Q���Z� �� -   $��optionsborder��
�������$��5q�$��1��$	��164��$
��Left=�$��100,250,0,0q�$	��Top=�$	��172��
 �����$ 0Q���Z� �� -	   $��tutorialborder��
�������$��5q�$��1��$	��164��$
��Left=�$��700,250,0,0q�$	��Top=�$	��172��
 �����$ 0Q���Z� �� -
   $��
quitborder��
�������$��5q�$��1��$	��164��$
��Left=�$��	400,0,0,0q�$	��Top=�$	��172��
 �����$ 0Q���Z� �� -   $��recordborder��
�������$��5q�$��1��$	��164��$
��Left=�$��400,500,0,0q�$	��Top=�$	��172��
 �����$ 0Q���Z� �� -   $
��logo��$��1��$��99��$
��Left=�$��6,6,0,0q�$
��Fill=�$	��Top=�$	��357��$'��!/KinectFitness;component/logo.png�� � ColumnDefinitions ��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �