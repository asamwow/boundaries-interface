/* Boundaries - Interfaces and Models
 * Original Version Written by Samuel Jahnke 2022
 *
 * This library is free software: you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.  If not, see
 * <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;

namespace boundaries.Models {

   public class Entropic {

      public const byte CellSize = Cell.CellSize;
      public const byte ChunkSize = Chunk.ChunkSize;

      public int id { get; set; }
      [JsonIgnore]
      public byte[] locations {
         get; set;
      }
      [JsonIgnore]
      public List<Appendage> appendages {
         get; set;
      }
      public byte energyCounter { get; set; }
      public byte autoAction { get; set; }
      public bool claimed { get; set; }
      public byte fuel { get; set; }
      public byte range { get; set; }
      public byte hp { get; set; }
      public Team team { get; set; }
      public enum Team { Red, Blue, Green, Yellow, Comet }
      public byte[] actionValues {
         get {
            if (actionQueue == null) {
               return new byte[0];
            }
            return actionQueue.ToArray();
         }
         set {
            if (actionQueue == null) {
               actionQueue = new Queue<byte>();
            } else {
               actionQueue.Clear();
            }
            for (int i = 0; i < value.Length; i++) {
               actionQueue.Enqueue(value[i]);
            }
         }
      }
      public bool ticked { get; set; }
      Queue<byte> actionQueue;

      private Entropic() {
         this.appendages = new List<Appendage>();
         this.locations = new byte[CellSize];
         claimed = false;
         ticked = true;
         actionQueue = new Queue<byte>();
         this.autoAction = 0;
         fuel = 99;
         range = 7;
         hp = 100;
         this.energyCounter = range;
      }

      public Entropic(byte x, byte y, Chunk chunk) : this() {
         SetLocation(x, y);
         chunk.entropics.Add(this);
         Cell cell = chunk.GetCellLocal(x, y);
         cell.settled = true;
         cell.entropic = true;
      }

      public Entropic(Entropic other) : this() {
         if (other != null) {
            Copy(other);
         }
      }

      public void Copy(Entropic other) {
         id = other.id;
         locations = (byte[])other.locations.Clone();
         appendages = other.appendages;
         claimed = other.claimed;
         actionValues = other.actionValues;
         ticked = other.ticked;
      }

      public void CloneAppendages(Entropic other) {
         appendages = new List<Appendage>();
         for (int i = 0; i < other.appendages.Count; i++) {
            appendages.Add(new Appendage(other.appendages[i]));
         }
      }

      public Entropic(int x, int y, Chunk chunk) : this((byte)x, (byte)y, chunk) {}

      public void SetLocation(int x, int y, bool isThere = true) {
         SetLocation((byte)x, (byte)y, isThere);
      }

      public void SetLocation(byte x, byte y, bool isThere = true) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Set Location index");
         }
         byte[] rowByte = new byte[1];
         rowByte[0] = locations[y];
         BitArray row = new BitArray(rowByte);
         row[x] = isThere;
         row.CopyTo(rowByte, 0);
         locations[y] = rowByte[0];
      }

      public bool GetLocation(int x, int y) { return GetLocation((byte)x, (byte)y); }

      public bool GetLocation(byte x, byte y) {
         if (x < 0 || x >= CellSize || y < 0 || y >= CellSize) {
            throw new Exception("Invalid Get Location index");
         }
         byte[] rowByte = new byte[1];
         rowByte[0] = locations[y];
         BitArray row = new BitArray(rowByte);
         return row[x];
      }

      public EntAction GetAction() {
         if (actionQueue.Count == 0) {
            return new EntAction(EntAction.Type.None);
         }
         return new EntAction(actionQueue.Peek());
      }

      public EntAction PopAction() {
         if (actionQueue.Count == 0) {
            return new EntAction(EntAction.Type.None);
         }
         return new EntAction(actionQueue.Dequeue());
      }

      public void GrowCellUp() {
         for (int y = CellSize - 1; y >= 0; y--) {
            for (int x = 0; x < CellSize; x++) {
               if (GetLocation(x, y)) {
                  SetLocation(x, y + 1);
               }
            }
         }
      }

      public void ClearLocation() { locations = new byte[CellSize]; }

      public void ClearActions() { actionQueue.Clear(); }

      public bool TryAddAction(EntAction action) {
         if (actionQueue.Count >= 32) {
            return false;
         }
         actionQueue.Enqueue(action.GetTypeValue());
         // string log = "Action Values: ";
         // for (int i = 0; i < actionValues.Length; i++) {
         //     log += actionValues[i] + ", ";
         // }
         // Console.WriteLine(log);
         return true;
      }

      public Appendage GetAppendage(int x, int y) {
         bool isBot = (y < 8);
         int absY = y;
         if (!isBot) {
            absY -= 8;
         }
         foreach (Appendage appendage in appendages) {
            if (appendage.GetLocation(x, absY) && appendage.isBottomCell == isBot) {
               return appendage;
            }
         }
         return null;
      }

      public Entropic(string hex) {
         id = (int)Convert.ToInt32(hex.Substring(0, 8), 16);
         energyCounter = (byte)Convert.ToInt32(hex.Substring(8, 2), 16);
         team = (Team)Convert.ToInt32(hex.Substring(10, 2), 16);
         hp = (byte)Convert.ToInt32(hex.Substring(12, 2), 16);
      }

      public void Write(Stream stream) {
         string digit = id.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         digit = energyCounter.ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         digit = ((int)team).ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         digit = hp.ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
      }
   }
}
