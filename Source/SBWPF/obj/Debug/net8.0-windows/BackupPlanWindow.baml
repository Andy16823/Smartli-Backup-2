   M S B A M L   `   `   `  ����@  <SBWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SBWPFSBWPF  5   	       SBWPF.BackupPlanWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:SBWPF     5   	   S OHandyControl, Version=3.5.1.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  5   	     � Title$  Backup Plan��5	   	   $	��450��6   $	��600��6*      CustomWindowStyle#���   66   .��5
      �  � RowDefinitions 5   
   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��* �6   6)   �� 5      $��40 �6   6*   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��1* �6#   60   �� 5      $��3* �6#   60   .+�5      �� $��5q�6   $��Backup Name��6#   $��Center=�66   6S   �� 5      -   5         ��Name$ backupNameTextbox��5        HandyControl.Themes.Theme   Skin$
 Dark=�61   $��1��6F   $��5q�6V   6c   5      � 5   
   $��1��6    5      �� 5      $��1* �6#   60   �� 5      $��3* �6#   60   .+�5      �� $��5q�6   $��Schedule��6#   $��Center=�63   6P   $  HandyControl.Controls.ComboBox  5       -   5          Name$ ScheduleCombobox��5       $
 Dark=�64   $��5q�6I   $��1��6T   6f   5!      �� 5"   
    �� Orientation$ 
Horizontal=�6   $��2��6.   $��Right=�6;   .+�5#      �� -   5         ��Name$ addFolderButton��5#      $	��120��6L   $��Add Directory��6[   $��5q�6s     ButtonPrimary#��[ 6~   )  #HandyControl.Controls.BorderElement   CornerRadius$ 5��6�   �� 5$      -   5        $ addFileButton��5$      $	��120��6H   $��Add File��6W   $��5q�6j   #��[ 6u   $ 5��6�   �� 5%      -   5        $ removeSelection��5%      $	��120��6M   $��Remove Selection��6\   $��5q�6w     ButtonDanger#��[ 6�   $ 5��6�   5&      �� 5'   
   -   5        	 ��Name$	 backupSources��5'      $��3��6*   $��5q�67   $
 Dark=�6B   
 �� View
 5(      �� 5)      .P�5*      ��  �� Width$
 auto��6%   $
��Name��62    �� DisplayMemberBinding 6@   �� *Name+6i   �� 5+      $
 auto��6%   $
��Path��62    6@   �� *Path+6h   �� 5,      $
 auto��6%   $
��Type��62    6@   �� *Type+6h   5-      5/      �� 50   
   $ 
Horizontal=�6   $��4��6.   $��Right=�6;   .+�51      �� -   5        $ createPlanButton��51      $��Save Backup Plan��6N   $��5q�6i   #��[ 6t   $ 15��6�   52      53      54      