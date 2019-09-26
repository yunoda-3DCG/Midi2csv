using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Midi2csv
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_OpenMidiFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "midi file(*.mid)|*.mid|すべてのファイル (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                MidiInfoCache(dialog);
            }
        }

        private void Button_Click_ExportCSV(object sender, RoutedEventArgs e)
        {
            if (previewTrackNum.Text == "")
            {
                MessageBox.Show("トラックが指定されていません。(error: track number is blank.)",
                                "Preview error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            var dialog = new SaveFileDialog();
            dialog.FileName = ".csv";
            dialog.InitialDirectory = @"c:\";
            dialog.Filter = "CSV file(*.csv)|*.csv|すべてのファイル|*.*";
            dialog.FilterIndex = 0;
            int timeFromStart = 0;
            if (dialog.ShowDialog() == true)
            {
                String delimiter = ",";
                StringBuilder sb = new StringBuilder();
                var targetTrackNum = Convert.ToInt32(previewTrackNum.Text) - 1;
                var targetNoteInfos = noteInfosList[targetTrackNum];
                if ((bool)isCriAtomFormat.IsChecked)//export Cry Atom Craft format
                {
                    sb.Append("Type").Append(delimiter);
                    sb.Append("CallbackId").Append(delimiter);
                    sb.Append("CallbackTag").Append(delimiter);
                    sb.Append("Comment").Append(delimiter);
                    sb.Append("EventEnableFlag").Append(delimiter);
                    sb.Append("EventEndTime").Append(delimiter);
                    sb.Append("EventStartTime").Append(delimiter);
                    sb.Append(Environment.NewLine);
                    foreach (var v in targetNoteInfos)
                    {
                        sb.Append("EventCallback").Append(delimiter);
                        sb.Append("").Append(delimiter);
                        sb.Append(ConvertEventParam2Meaning(v.eventType, v.eventByte, v.eventParam)).Append(delimiter);
                        sb.Append(v.eventType.ToString()).Append(delimiter);
                        sb.Append("True").Append(delimiter);
                        sb.Append("0").Append(delimiter);
                        sb.Append(ConvertDeltatime2Time(v.deltaTime)).Append(delimiter);
                        sb.Append(Environment.NewLine);
                    }
                }
                else//basic format export
                {
                    foreach (var v in targetNoteInfos)
                    {
                        sb.Append(v.noteID).Append(delimiter);
                        sb.Append(v.deltaTime).Append(delimiter);
                        sb.Append(v.eventType.ToString()).Append(delimiter);
                        sb.Append(ConvertEventParam2Meaning(v.eventType, v.eventByte, v.eventParam)).Append(delimiter);
                        sb.Append(Environment.NewLine);
                    }
                }
                FileStream fs = new FileStream(dialog.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(sb);
                sw.Close();
                fs.Close();
            }
            string ConvertEventParam2Meaning(EventType eventType, byte eventTypeByte, List<byte> bytes)
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                string str = "";
                switch (eventType)
                {
                    case EventType.NoteOff:
                    case EventType.NoteOn:
                    case EventType.PolyphonicPressure:
                    case EventType.ControllChange:
                    case EventType.ProgramChange:
                    case EventType.ChannelPressure:
                    case EventType.PitchBend:
                        str += eventTypeByte.ToString("x2")[1] + " ";
                        foreach (var v in bytes)
                        {
                            str += v.ToString("x2");
                        }
                        return str;
                    case EventType.META_sequenceNum:
                    case EventType.META_text:
                    case EventType.META_copyrights:
                    case EventType.META_title:
                    case EventType.META_instrument:
                    case EventType.META_lyrics:
                    case EventType.META_marker:
                    case EventType.META_queuePoint:
                    case EventType.META_tone:
                    case EventType.META_soundSource:
                    case EventType.META_channelPrefix:
                    case EventType.META_port:
                    case EventType.META_trackEnd:
                    case EventType.META_tempo:
                    case EventType.META_smpteOffset:
                    case EventType.META_beat:
                    case EventType.META_key:
                    case EventType.META_specialMetaEvent:
                    case EventType.ExclusiveMessage:
                        str = sjisEnc.GetString(bytes.ToArray());
                        return str;
                }
                return "";
            }
            int ConvertDeltatime2Time(int deltatime)
            {
                if ((bool)convertDeltatime2Time.IsChecked)
                {
                    timeFromStart += deltatime;
                    return timeFromStart;
                }
                else
                    return deltatime;
            }
        }

        private void ButtonClick_TrackPreview(object sender, RoutedEventArgs e)
        {
            PreviewNoteInfos();
        }

        private void Button_Click_ChangeLanguage(object sender, RoutedEventArgs e)
        {
            /*if (languageChoice.SelectedIndex == 0)
                ResourceService.Current.ChangeCulture("en-US");
            else
                ResourceService.Current.ChangeCulture("ja-JP");
        */
        }

        public enum EventType : int
        {
            //MIDIevents
            NoteOn,
            NoteOff,
            PolyphonicPressure,
            ControllChange,
            ProgramChange,
            ChannelPressure,
            PitchBend,
            //SysExEvents
            ExclusiveMessage,
            //MetaEvents
            META_sequenceNum,
            META_text,
            META_copyrights,
            META_title,
            META_instrument,
            META_lyrics,
            META_marker,
            META_queuePoint,
            META_tone,
            META_soundSource,
            META_channelPrefix,
            META_port,
            META_trackEnd,
            META_tempo,
            META_smpteOffset,
            META_beat,
            META_key,
            META_specialMetaEvent,
            //
            Unknown,
        }

        public class NoteInfo
        {
            public int noteID { get; set; }
            public int deltaTime { get; set; }
            public EventType eventType { get; set; }
            public byte eventByte;
            public List<byte> eventParam { get; set; }
        }

        public int index = 0;
        List<List<NoteInfo>> noteInfosList = new List<List<NoteInfo>>();

        private void MidiInfoCache(OpenFileDialog targetMidi)
        {
            var midiBinaryData = new BinaryReader(targetMidi.OpenFile());

            /////check whether midi file or not
            var chunktype_header = midiBinaryData.ReadBytes(4);
            var headStrings = chunktype_header[0].ToString("x2")
                + chunktype_header[1].ToString("x2")
                + chunktype_header[2].ToString("x2")
                + chunktype_header[3].ToString("x2");
            if (headStrings != "4d546864")
            {
                MessageBox.Show("Midiファイルとして読み込めません。(error: header chunk is wrong.)",
                    "読み込みエラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            midiBinaryData.ReadBytes(4); // this part read the data length of midi file(no need to cache)

            ///// this part inserts track num, midi format and delta time into basic infomation box 
            var format = midiBinaryData.ReadBytes(2);
            midiFormat.Text = format[1].ToString("x2");
            var track = midiBinaryData.ReadBytes(2);
            midiTrack.Text = (track[0] + track[1]).ToString();
            var unit = midiBinaryData.ReadBytes(2);
            midiTimeUnit.Text = track[1].ToString() + (track[0] >= 0 ? "分解能/四分音符" : "分解能/1フレーム");
            /////

            noteInfosList.Clear();

            /////chach the info of each track data
            var trackCount = track[0] + track[1];
            for (int i = 0; i < trackCount; i++)
            {
                var chunktype_Track = midiBinaryData.ReadBytes(4);
                var trackStrings = chunktype_Track[0].ToString("x2")
                    + chunktype_Track[1].ToString("x2")
                    + chunktype_Track[2].ToString("x2")
                    + chunktype_Track[3].ToString("x2");
                if (trackStrings != "4d54726b")
                {
                    MessageBox.Show("Midiファイルとして読み込めません。(error: track chunk is wrong.)",
                        "読み込みエラー",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                var dataLength = midiBinaryData.ReadBytes(4);
                int byteCount = Convert.ToInt32(dataLength[0].ToString("x2")
                    + dataLength[1].ToString("x2")
                    + dataLength[2].ToString("x2")
                    + dataLength[3].ToString("x2"),
                    16
                    );
                //Debug.Print("[Track start] track length is " + byteCount);
                int counter = 0;
                List<byte> byteCopys = new List<byte>();
                byteCopys.AddRange(midiBinaryData.ReadBytes(byteCount));
                index = 0;
                List<NoteInfo> noteInfos = new List<NoteInfo>();
                EventType currentEv = EventType.Unknown;
                byte runningStatus = 0;
                while (counter < byteCount)
                {
                    EventType ReadEventType(int targetIndex)
                    {
                        if ((byteCopys[targetIndex] & (byte)0x80) == 0)
                        {
                            return currentEv;
                        }
                        index++;
                        runningStatus = byteCopys[targetIndex];
                        switch (byteCopys[targetIndex].ToString("x2")[0])
                        {
                            case '8':
                                return EventType.NoteOff;
                            case '9':
                                return EventType.NoteOn;
                            case 'a':
                                return EventType.PolyphonicPressure;
                            case 'b':
                                return EventType.ControllChange;
                            case 'c':
                                return EventType.ProgramChange;
                            case 'd':
                                return EventType.ChannelPressure;
                            case 'e':
                                return EventType.PitchBend;
                            case 'f':
                                return ReadEventType_META(targetIndex);
                        }
                        return EventType.Unknown;
                    }
                    EventType ReadEventType_META(int targetIndex)
                    {
                        if (byteCopys[targetIndex].ToString("x2")[1] == '0'
                            || byteCopys[targetIndex].ToString("x2")[1] == '7')
                        {
                            return EventType.ExclusiveMessage;
                        }
                        else
                        {
                            index++;
                            switch (byteCopys[targetIndex + 1].ToString("x2"))
                            {
                                case "00":
                                    return EventType.META_sequenceNum;
                                case "01":
                                    return EventType.META_text;
                                case "02":
                                    return EventType.META_copyrights;
                                case "03":
                                    return EventType.META_title;
                                case "04":
                                    return EventType.META_instrument;
                                case "05":
                                    return EventType.META_lyrics;
                                case "06":
                                    return EventType.META_marker;
                                case "07":
                                    return EventType.META_queuePoint;
                                case "08":
                                    return EventType.META_tone;
                                case "09":
                                    return EventType.META_soundSource;
                                case "20":
                                    return EventType.META_channelPrefix;
                                case "21":
                                    return EventType.META_port;
                                case "2f":
                                    return EventType.META_trackEnd;
                                case "51":
                                    return EventType.META_tempo;
                                case "54":
                                    return EventType.META_smpteOffset;
                                case "58":
                                    return EventType.META_beat;
                                case "59":
                                    return EventType.META_key;
                                case "7f":
                                    return EventType.META_specialMetaEvent;
                            }
                        }
                        return EventType.Unknown;
                    }
                    List<byte> ReadEventInfos(int targetIndex)
                    {
                        List<byte> t = new List<byte>();
                        int size = 0;
                        switch (currentEv)
                        {
                            case EventType.ProgramChange:
                            case EventType.ChannelPressure:
                                t.Add(byteCopys[targetIndex]);
                                size = 1;
                                break;
                            case EventType.NoteOff:
                            case EventType.NoteOn:
                            case EventType.PolyphonicPressure:
                            case EventType.ControllChange:
                            case EventType.PitchBend:
                                t.Add(byteCopys[targetIndex]);
                                t.Add(byteCopys[targetIndex + 1]);
                                size = 2;
                                break;
                            case EventType.META_sequenceNum:
                            case EventType.META_text:
                            case EventType.META_copyrights:
                            case EventType.META_title:
                            case EventType.META_instrument:
                            case EventType.META_lyrics:
                            case EventType.META_marker:
                            case EventType.META_queuePoint:
                            case EventType.META_tone:
                            case EventType.META_soundSource:
                            case EventType.META_channelPrefix:
                            case EventType.META_port:
                            case EventType.META_trackEnd:
                            case EventType.META_tempo:
                            case EventType.META_smpteOffset:
                            case EventType.META_beat:
                            case EventType.META_key:
                            case EventType.META_specialMetaEvent:
                            case EventType.ExclusiveMessage:
                                size = ConvertVariable2int(byteCopys, targetIndex);
                                for (int p = 0; p < size; p++)
                                    t.Add(byteCopys[targetIndex + 1 + p]);
                                break;
                        }
                        //Debug.Print(targetIndex +":" + byteCopys[targetIndex - 1].ToString("x2") + byteCopys[targetIndex].ToString("x2") + ": " + size);
                        index += size;
                        return t;
                    }

                    var tempNote = new NoteInfo()
                    {
                        noteID = noteInfos.Count,
                        deltaTime = ConvertVariable2int(byteCopys, index),
                        eventType = currentEv = ReadEventType(index),
                        eventByte = runningStatus,
                        eventParam = ReadEventInfos(index),
                    };
                    noteInfos.Add(tempNote);
                    if (tempNote.eventType == EventType.META_trackEnd)
                        break;
                    counter++;
                }
                noteInfosList.Add(noteInfos);
            }
        }

        private void PreviewNoteInfos()
        {
            if (previewTrackNum.Text == "")
            {
                MessageBox.Show("トラックが指定されていません。(error: track number is blank.)",
                                "Preview error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            var targetTrackNum = Convert.ToInt32(previewTrackNum.Text) - 1;
            if (targetTrackNum >= noteInfosList.Count || targetTrackNum < 0)
            {
                MessageBox.Show("トラック数を超えています。(error: target track number is wrong.)",
                        "Preview error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                return;
            }
            if ((bool)convertVolume00NoteOn.IsChecked)
            {
                for (int i = 0; i < noteInfosList[targetTrackNum].Count; i++)
                {
                    if (noteInfosList[targetTrackNum][i].eventType == EventType.NoteOn
                        && noteInfosList[targetTrackNum][i].eventParam[1] == 0)
                        noteInfosList[targetTrackNum][i].eventType = EventType.NoteOff;
                    else if (noteInfosList[targetTrackNum][i].eventType == EventType.NoteOff)
                        break;
                }
            }
            listView.DataContext = noteInfosList[targetTrackNum];
            totalNoteNum.Text = noteInfosList[targetTrackNum].Count.ToString();
        }

        private void textBoxPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
        private void textBoxPrice_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// if the target may be variable length, should use this method.
        /// </summary>
        private int ConvertVariable2int(List<byte> data, int startIndex)
        {
            const int max_variable_size = 5;
            const int max_int_size = 4;
            int size = 1;
            for (int i = 0; (data[startIndex + i] & (byte)0x80) > 0; i++)
            {
                size++;
            }
            if (size > max_variable_size)
            {
                return 0;
            }
            //Debug.Print(startIndex + ": " + data[startIndex].ToString("x2"));

            byte[] bits = new byte[max_int_size * 8];
            int pos = bits.Length - 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    byte comp = (byte)(1 << j);
                    if ((data[startIndex + size - 1 - i] & comp) > 0)
                        bits[pos] = 1;
                    pos--;
                }
            }
            index += size;
            string text_DEBUG = "";
            for (int i = 0; i < bits.Length; i++)
            {
                text_DEBUG += bits[i];
            }
            //Debug.Print(text_DEBUG.ToString()+": "+ Convert.ToInt32(text_DEBUG, 2));
            return Convert.ToInt32(text_DEBUG, 2);
        }
    }
}
