   M S B A M L   `   `   `  ����  
Aplproject    Aplproject.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��800��6   $
��1150��6)   .��5      � .+�5   
   �� -   5         ��Name$ loadImageButton��5      $��
Load Image��6*   $
��Left=�6?   $	��Top=�6Z   $��
752,33,0,0q�6r   6�   �� 5   
   -   5        $	 Run��5      $��Run functions��6   $
��Left=�66   $	��Top=�6Q   $��741,241,0,0q�6i   6~   �� 5	   
   $��28,30,645,403q�6    J� RenderTransformOrigin$ 0.5,0.5&�6.   z�5
      V� 5      .~�5      �� �� 5      �� 5       �� Angle$ -0.271Q�6&   T� 5      5      .��5      �� -   5         ��Name$ displayImage��5      $��Center=�6*   $��Center=�6G   $��Uniform=�6b   5      �� 5   
   -   5         ��Name$ asmFunctionCheckbox��5   x   $��Enable Asm function��6   $
��Left=�61   $	��Top=�6L   $��
604,81,0,0q�6d   6x   �� 5   
   -   5        $ cppFunctionCheckbox��5   x   $��Enable C++ function��6   $
��Left=�61   $	��Top=�6L   $��
884,81,0,0q�6d   6x   �� 5   
   $!��Number of consecutive runs:��6   $
��Left=�66   $	��Top=�6Q   $��575,134,0,0q�6i   �� 5   
   $��Enter threshold:��6   $
��Left=�6+   $	��Top=�6F   $��875,134,0,0q�6^   �� 5   
   -   5        $��
RunTextBox��5      $
��Left=�6$   $	��Top=�6?   $��613,183,0,0q�6{   $��86��6�   $��22��6�   6�   �� 5   
   -   5        $��ThresholdTextBox��5      $
��Left=�6*   $	��Top=�6E   $��888,183,0,0q�6�   $��86��6�   $��22��6�   6�   �� 5   
   $��28,421,645,12q�6   $ 0.5,0.5&�6.   z�5      V� 5      .~�5      �� �� 5      �� 5      $ -0.271Q�6&   T� 5       5!      .��5#      �� -   5        $ binarizedImage��5#      $��Center=�6,   $��Center=�6I   $��Uniform=�6d   5$      �� 5%   
   $��Statistics:��6   $
��Left=�6&   $	��Top=�6A   $��575,427,0,0q�6Y   � 5&   
   -	   5        $��progressBar��5&      $
��Left=�6)   $	��Top=�6D   $��20��6\   $��695,300,0,0q�6h   $	��181��6}   �� 5'   
   $��Original Image:��6   $
��Left=�6*   $	��Top=�6E   $��27,0,0,0q�6]   �� 5(   
   $��Binarized Image:��6   $
��Left=�6+   $	��Top=�6F   $��
27,392,0,0q�6^   �� 5)   
   $��Progress bar:��6   $
��Left=�6(   $	��Top=�6C   $��739,269,0,0q�6[   �� 5*   
   -
   5         ��Name$ CppStatisticsTextblock��5*      $
��Left=�64   $	��Top=�6O   $��13(�6g   $��575,494,0,0q�6u   $	��243��6�   $	��239��6�   �� 5+   
   -   5        $ AsmStatisticsTextblock��5+      $
��Left=�64   $	��Top=�6O   $��13(�6g   $��888,494,0,0q�6u   $	��243��6�   $	��239��6�   5,      5-      