   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   ^    KinectFitness!   KinectFitness.StartupWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   ^   -   5          � Title$  Kinect Fitness��5   -   $	��800��6D   $
��1300��6Q   .��5   
   � $��
10,0,10,10q�6   ��5   
   �� 5       �� ImageSource$2 ,/KinectFitness;component/backgroundImage.jpg��6   .+�5   
   �� $��0q�6   .+�5      �� -   5        $��playButtonImg��5   l    �� Left$	 422��6   
 �� Top$	 456��6&   $��1��67   $	��164��6D   $
��Left=�6Q   $
��Fill=�6�   $	��Top=�6�   $	��172��6�   $-��'/KinectFitness;component/playbutton.png��6�   6�   �� 5      -   5        $��optionsButtonImg��5   _   $	 422��6   $	 194��6&   $	��164��67   $
��Left=�6D   $
��Fill=�6w   $	��Top=�6�   $	��172��6�   $0��*/KinectFitness;component/optionsbutton.png��6�   $��1��6�   �� 5      -   5        $��quitButtonImg��5   m   $	 732��6   $	 456��6&   $��1��67   $	��164��6D   $
��Left=�6Q   $
��Fill=�6�   $	��Top=�6�   $	��172��6�   $-��'/KinectFitness;component/quitbutton.png��6�   6�   �� 5      -   5        $��recordButtonImg��5   m   $	 732��6   $	 194��6&   $��1��67   $	��164��6D   $
��Left=�6Q   $
��Fill=�6�   $	��Top=�6�   $	��172��6�   $/��)/KinectFitness;component/recordbutton.png��6�   6�   �� 5      -   5        $��
playborder��5   �   $	 422��6   $	 456��6'   
�������$��5q�6L   $��1��6`   $	��164��6m   $
��Left=�6z   $	��Top=�6�   $	��172��6�    J� OpacityMask
 ����� J� Opacity$ 0Q�6�   ��6�   Z� 6�   �� 5      -   5        $��optionsborder��5   �   $	 422��6   $	 194��6'   
�������$��5q�6L   $��1��6`   $	��164��6m   $
��Left=�6z   $	��Top=�6�   $	��172��6�   
 �����$ 0Q�6�   ��6�   Z� 6�   �� 5      -   5        $��
quitborder��5   �   $	 732��6   $	 456��6'   
�������$��5q�6L   $��1��6`   $	��164��6m   $
��Left=�6z   $	��Top=�6�   $	��172��6�   
 �����$ 0Q�6�   ��6�   Z� 6�   �� 5      -	   5        $��recordborder��5   �   $	 732��6   $	 194��6'   
�������$��5q�6L   $��1��6`   $	��164��6m   $
��Left=�6z   $	��Top=�6�   $	��172��6�   
 �����$ 0Q�6�   ��6�   Z� 6�   �� 5      -
   5        $
��logo��5   Y   $��1��6   $��99��6!   $
��Left=�6-   $��6,6,0,0q�6H   $
��Fill=�6e   $	��Top=�6t   $	��357��6�   $'��!/KinectFitness;component/logo.png��6�   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5   
   -   5        $��kinectSensorChooser1��5   \   $��1��6!   $
��Left=�6.   $��	6,525,0,0q�6I   $	��Top=�6x   $	��248��6�   $	��234��6�   $
 -519��6�   $	 998��6�   �� 5   
   -   5        $��	rightHand��5   X   $��56��6   $
��Left=�6   $	 204��67   $ 6��6I   $
��Fill=�6i   $	��Top=�6x   $��57��6�   $/��)/KinectFitness;component/right%20hand.gif��6�    K� ZIndex$ 5��6�   �� 5   
   -   5        $��rightHandProgressBar��5   M   $��15��6   $
��Left=�6    $��0,50,0,0q�6;   $	��Top=�6{   $��57��6�   ���	#FF0BD40B$ 5��6�   ��6i   Z� 6�   5"      5#      5$      