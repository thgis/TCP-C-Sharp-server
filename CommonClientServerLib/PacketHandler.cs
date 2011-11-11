using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClientServerLib
{
    public class PacketHandler
    {
        /// <summary>
        /// Delegate to a method that handles DRM byte out of sync event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ByteOutOfSyncEventHandler(object sender, EventArgs args);

        public delegate void CompletePacketReceivedEventHandler(object sender, CompletePacketReceivedArgs args);

        /// <summary>
        /// Event that is raised when a byte is detected to be out of sync.
        /// </summary>
        public event ByteOutOfSyncEventHandler ByteOutOfSync;

        public event CompletePacketReceivedEventHandler CompletePacketReceived;

        private StreamsStatus _status;
        private List<byte> _completePacket;

        #region --enumeration--
        /// <summary>
        /// Emuneration of the different states that the reveiving stream can be in.
        /// </summary>
        private enum StreamsStatus : byte
        {
            UNSYNC,
            FIRST_END_RECEIVED,
            SECOND_END_RECEIVED,
            FIRST_HEADER_RECEIVED,
            RECEIVING_DATA,
            HOLE_PACKET_RECEIVED,
        }
        #endregion

        public PacketHandler()
        {
            _status = StreamsStatus.UNSYNC;
        }

        public void DetectPacket(byte dataByte)
        {
            // Recognize header fields and change status
            if (_status == StreamsStatus.UNSYNC && dataByte == 0x02)
            {
                _status = StreamsStatus.FIRST_HEADER_RECEIVED;
                _completePacket = new List<byte>();
                _completePacket.Add(dataByte);
            }
            else if (_status == StreamsStatus.FIRST_HEADER_RECEIVED && dataByte == 0x10 || _status == StreamsStatus.RECEIVING_DATA && dataByte == 0x10)
            {
                _status = StreamsStatus.FIRST_END_RECEIVED;
                _completePacket.Add(dataByte);
            }
            else if (_status == StreamsStatus.FIRST_END_RECEIVED && dataByte == 0x03)
            {
                _status = StreamsStatus.SECOND_END_RECEIVED;
                _completePacket.Add(dataByte);
            }
            else if (_status == StreamsStatus.FIRST_END_RECEIVED && dataByte == 0x10)
            {
                _status = StreamsStatus.RECEIVING_DATA;
            }
            else
            {
                switch (_status)
                {
                    case StreamsStatus.FIRST_HEADER_RECEIVED:
                        _completePacket.Add(dataByte);
                        _status = StreamsStatus.RECEIVING_DATA;
                        break;
                    case StreamsStatus.RECEIVING_DATA:
                        _completePacket.Add(dataByte);
                        break;
                    case StreamsStatus.SECOND_END_RECEIVED:
                        List<Byte> _temp = new List<Byte>(_completePacket.Count - 3);
                        _temp.AddRange(_completePacket.GetRange(1,_completePacket.Count - 3));

                        CompletePacketReceivedArgs args = new CompletePacketReceivedArgs(_temp);
                        if (CompletePacketReceived != null)
                            CompletePacketReceived(this, args);

                        _status = StreamsStatus.HOLE_PACKET_RECEIVED;
                        resetStream();
                        break;

                    default:
                        resetStream();
                        break;
                }
            }
        }

        /// <summary>
        /// Method that resets the stream status
        /// </summary>
        private void resetStream()
        {
            if (_status != StreamsStatus.HOLE_PACKET_RECEIVED)
            {
                if (ByteOutOfSync != null)
                    ByteOutOfSync(this, new EventArgs());
            }

            _status = StreamsStatus.UNSYNC;
            _completePacket = null;
        }
    }
}
