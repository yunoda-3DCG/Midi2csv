midi2csv
====

 With midi2csv you can write down the midi file into csv file. This tool converts the binary data into text data which you can read easily(ex.60 9c 50 => Deltatime, 60 : EventType, Channel 13 Note On: Velocity, 50...)

## Description
 When you have to analyze midi file, this app can help you to understand the contents of file in the text base.
In the binary base, it will be difficult to read the contents and understand what the data array means, also in the DAW(Digital Audio Workstation), the DTM software does not show all the details of the midi files.  
 Therefore I created this progroms in order to analyse the midi file and to check the contents in the format to read as easy as you can.
In addtion to this purpose, this app aim the use of the sound middle ware"ADX2LE"(https://game.criware.jp/products/adx2-le/). In the first step, this app will aim to put callback markers easily in the ADX2LE editor.


## Usage
![usage_midi2csv_ver1 0](https://user-images.githubusercontent.com/50200315/65660122-a1f63a80-e068-11e9-8bf0-34d05fded68e.png)
 
1- Select midi file.  
2- Input the track number you want to see or export (this number is essential to export csv file).  
3- Push preview button.  
(3.5- Check some option.)  
4- Push export button.  
 
## Install
 No need to install. Please download .exe file from my release page. This app contains no certificate, so your PC may send some warnings.

## Licence
Copyright(c) 2019 Yu Noda
Released under the MIT license
https://opensource.org/licenses/mit-license.php

## Author
Yu Noda(https://github.com/yarikomiVR) twitter:(https://twitter.com/yarikomivr_com)
