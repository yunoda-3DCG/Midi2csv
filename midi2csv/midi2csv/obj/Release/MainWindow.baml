   M S B A M L   `   `   `  ���� C  ?midi2csv, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Midi2csvMidi2csv      Midi2csv.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Midi2csv       � Title$  midi2csv��$	��450��$	��800�� � 
ResizeMode$ NoResize=� � Topmost .  � Icon$# resource/appicon_midi2csv.ico��.���  � ColumnDefinitions �� $
��290* ��� $
��107* �.+��� -   $��export into csv file��$��4,369,10,20q�$��1���� $	��125�� �� TextWrapping$
 Wrap=�$����Basic function: ver1.0.0
1. Read midi file and analyze the total tracks, midi format and delta time unit.
2. Preview the contents before export csv file.
3. Convert volume 0 Note On to Note Off(if you convert once, you cannot undo it. Then you have to read file again.)  
4. Can export with the real time from the track start.
5. "experimental" CriAtomCraft format. This format is for using sound middle ware "ADX2LE".
��$	��Top=�$��	10,10,9,0q��� -   $��Select midi file��$��
0,103,10,0q�$	��Top=�$��32��$��1��$��Right=�$	��200��� -    �Name$ Preview��$��261,135,10,20q�.+��� -    ��Name$ listView��$��	0,26,-1,0q�����  �� View �� .P��� $��ID��	 �� Width$	 50��
 �� DisplayMemberBinding
 �� *noteID+�� $��	Deltatime��$	 50��
 �� *	deltaTime+�� $��
Event Type��$		 100��
 �� *	eventType+�� $��Event Contents��$	 50��
 �� *
eventParam+�� �� $��Preview��$
��Left=�$	��Top=�$	��309��$
��Bold"�� -   $ MidiFileInfomation��$
��Left=�$��10,135,0,20q�$	��246��.+��� $��midi file information��$
��Left=�$	��Top=�$	��222��$
��Bold"��� -    ��Name$ MidiFileBasicInfo_LIST��$��0,26,0,0q�.@�� -   $ 	MidiTrack��$	��222��$��46��.+��� $
��Wrap=�$��Total tracks��$��	0,0,10,22q�$��Center=��� -    ��Name$ 	midiTrack��$
 Wrap=�$��	0,24,10,0q�� -	   $ 
MidiFormat��$	��222��$��46��.+��� $
��Wrap=�$��midi format��$��0,4,0,26q�$
��Left=�$��Center=�$	��212���� -
   $ 
midiFormat��$
 Wrap=�$��	0,24,10,0q�� -   $ 	Deltatime��$	��222��$��46��.+��� $
��Wrap=�$��Deltatime Unit��$��0,4,0,26q�$
��Left=�$��Center=�$��16��$	��212���� -   $ midiTimeUnit��$
 Wrap=�$��	0,24,10,0q� J� 	AllowDrop .  � -   $ TotalNoteNum��$	��Top=�$��
4,193,10,0q�$��1��.+��� $
��Wrap=�$��0,0,0,23q�$��Center=�$��20��$
��Left=�$	��160���� $�Total notes���� -   $ totalNoteNum��$
 Wrap=�$��	0,20,10,0q� J� RenderTransformOrigin$ 
0.29,0.478&�� -   $ PreviewTrackNum��$��48��$	��Top=�$��1��$��
4,140,10,0q�.+��� $
��Wrap=�$��Preview Track No.��$��0,0,0,26q�$��22��$��Center=�$
��Left=�$	��190���� -   $ previewTrackNum�� �� IsInputMethodEnabled .  $
 Wrap=�$��
0,22,102,5q��� -   $��Preview��$��
103,22,0,5q��� -    ��Name$ isCriAtomFormat��$��CriAtomCraft format��$��1��$
��Left=�$��	4,349,0,0q�$	��Top=�$	��200���� -   $ convertDeltatime2Time��$)��#With the real time from track start��$��1��$
��Left=�$��	4,329,0,0q�$	��Top=�$	��200���� -   $ convertVolume00NoteOn��$%��Convert Vol.0 NoteON to NoteOff��$��1��$
��Left=�$��	4,241,0,0q�$	��Top=�$	��200��