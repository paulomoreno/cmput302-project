   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   G    KinectFitness%   KinectFitness.SelectLevelWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   G   -   5          � Title$  SelectLevelWindow��5      $��Verdana)�6   .��5
   
   � ��5   
   �� 5       �� ImageSource$2 ,/KinectFitness;component/backgroundImage.jpg��6   .+�5   
   �� -   5        $��title��5   O   $	��150��6   $
��Left=�6!   $��	355,6,0,0q�6<   $��SELECT EXERCISE��6\   $	��Top=�6s   $	��778��6�   
�������$��72(�6�   �� 5      -   5        $��levels��5   V   $	��286��6   $
��Left=�6&   $��544,544,0,0q�6A   $	��Top=�6d   $	��372��6|   6�   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5      -   5        $��kinectSensorChooser1��5   T   $
��Left=�6%   $��
12,466,0,0q�6@   $	��Top=�6p   $	��256��6�   $	��234��6�   �� 5   
   $��0q�6   .+�5      �� -   5        $��
intenseImg��5      $	��206��6&   $
��Left=�63   $
��Fill=�6O   $	��Top=�6^   $	��316��6v   $5��//KinectFitness;component/intenseCardioImage.png��6�    J� Opacity$ 1Q�6�    �� Left$	 942��6�   
 �� Top$	 164��6�   6�   �� 5      -   5        $��moderateImg��5      $
��Fill=�6'   $	��316��66   $6��0/KinectFitness;component/moderateCardioImage.png��6B   $	 544��6}   $	 164��6�   $	��206��6�   6�   �� 5      -   5        $��	warmUpImg��5      $	��206��6%   $
��Left=�62   $
��Fill=�6O   $	��Top=�6^   $	��316��6v   $.��(/KinectFitness;component/WarmUpImage.png��6�   $	 131��6�   $	 164��6�   6�   �� 5      -   5        $��warmUpImgBorder��5      
�������$��5q�6@   $��1��6T   $	��206��6a   $
��Left=�6n   $	��Top=�6�   $	��316��6�    J� OpacityMask
 �����$ 0Q�6�   $	 131��6�   $	 164��6�   ��6�   Z� 6�   �� 5      -	   5        $��moderateImgBorder��5      
�������$��5q�6B   $��1��6V   $	��206��6c   $
��Left=�6p   $	��Top=�6�   $	��316��6�   
 �����$ 0Q�6�   $	 544��6�   $	 164��6�   ��6�   Z� 6�   �� 5      -
   5        $��intenseImgBorder��5      
�������$��5q�6A   $��1��6U   $	��206��6b   $
��Left=�6o   $	��Top=�6�   $	��316��6�   
 �����$ 0Q�6�   $	 942��6�   $	 164��6�   ��6�   Z� 6�   �� 5      -   5        $��	rightHand��5   \   $��56��6   $
��Left=�6    $	 204��6;   $ 6��6M   $
��Fill=�6m   $	��Top=�6|   $��57��6�   $/��)/KinectFitness;component/right%20hand.gif��6�    K� ZIndex$ 5��6�   �� 5      -   5        $��rightHandProgressBar��5   Q   $��15��6   $
��Left=�6$   $��0,50,0,0q�6?   $	��Top=�6   $��57��6�   ���	#FF0BD40B$ 5��6�   ��6m   Z� 6�   �� 5      -   5        $��backButtonImg��5   B   $ 32��6   $ 12��6%   $	��100��65   $
��Fill=�6W   $	��100��6f   $-��'/KinectFitness;component/backbutton.gif��6r   �� 5      -   5        $��backButtonHoverImg��5   B   $ 32��6   $ 12��6%   $	��100��65   $
��Fill=�6\   $	��100��6k   $2��,/KinectFitness;component/backbuttonhover.gif��6w   $ 0Q�6�   6�   5      5      5!      