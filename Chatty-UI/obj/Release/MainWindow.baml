   M S B A M L   `   `   `  ����   	Chatty-UI    Chatty_UI.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  Chatty-Client��$	��600��$	��800��.��� .+���  �� Padding$ 20q�._��� .+��  � ColumnDefinitions �� $��2* ��� $��* �.+��� .+��� -    ��Name$ ConnectBanner��$��25 0 0 0q�$��Not Connected��$��16(� �� Padding$ 0 0 0 10q�$
��Bold"��� $��25 0 0 0q�$��Thread: ��$ 0 0 0 10q��� -    ��Name$ ConnectButton��$��20 10 25 10q�$��40��$��Connect��$��1���� -   $ ChatBox��$	��280��$	��700��
�������$�� ��$��14(�$
��Wrap=��� $��	Message: ��$��	25 10 0 0q��  �� $��2* ��� $��* �.+��� -    ��Name$ 
MessageBox��$��0��$��40��$��5q�$��16(�$��
25 10 20 0q�
��������� -   $ 
SendButton��$
��Send��$��1��$��
20 10 25 0q�� $��20q� �� $
��238* ��� $
��238* ��� $	��27* ��� $
��210* �.+��� -   $��5,5,5,5q�$
��Exit��$��0��$��40���� -   $��5,5,5,5q�$��Members��$��1��$��40���� -   $��5,5,5,5q�$��
Clear Chat��$��2��$��40��$��2��