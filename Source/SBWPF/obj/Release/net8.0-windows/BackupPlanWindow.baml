   M S B A M L   `   `   `  ���� @  <SBWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SBWPFSBWPF      SBWPF.BackupPlanWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:SBWPF     S OHandyControl, Version=3.5.1.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol    � Title$  Backup Plan��$	��450��$	��600��   CustomWindowStyle#���   .���  � RowDefinitions �� $��40 ��� $��40 ��� $��40 ��� $��* ��� $��40 �.+�� $��0�� � ColumnDefinitions �� $��1* ��� $��3* �.+��� $��5q�$��Backup Name��$��Center=��� -    ��Name$ backupNameTextbox��  HandyControl.Themes.Theme   Skin$
 Dark=�$��1��$��5q�� $��1�� �� $��1* ��� $��3* �.+��� $��5q�$��Schedule��$��Center=�$  HandyControl.Controls.ComboBox  -     Name$ ScheduleCombobox��$
 Dark=�$��5q�$��1����  �� Orientation$ 
Horizontal=�$��2��$��Right=�.+��� -    ��Name$ addFolderButton��$	��120��$��Add Directory��$��5q�  ButtonPrimary#��[ )  #HandyControl.Controls.BorderElement   CornerRadius$ 5���� -   $ addFileButton��$	��120��$��Add File��$��5q�#��[ $ 5���� -   $ removeSelection��$	��120��$��Remove Selection��$��5q�  ButtonDanger#��[ $ 5���� -   	 ��Name$	 backupSources��$��3��$��5q�$
 Dark=�
 �� View
 �� .P���  �� Width$
 auto��$
��Name�� �� DisplayMemberBinding �� *Name+�� $
 auto��$
��Path�� �� *Path+�� $
 auto��$
��Type�� �� *Type+�� $ 
Horizontal=�$��4��$��Right=�.+��� -   $ createPlanButton��$��Save Backup Plan��$��5q�#��[ $ 15��