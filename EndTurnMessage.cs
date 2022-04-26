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
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace boundaries.Models {

   public class EndTurnMessage : UnityMessage {

      public bool processing;

      public bool requested;

      public EndTurnMessage(bool processing, bool requested = false) : base(0, 0) {
         this.processing = processing;
         this.requested = requested;
         messageType = 14;
      }

      public EndTurnMessage(string hex) : base(hex) {
         if (Convert.ToInt32(hex.Substring(18, 1), 2) == 0) {
            processing = false;
         } else {
            processing = true;
         }
         if (Convert.ToInt32(hex.Substring(19, 1), 2) == 0) {
            requested = false;
         } else {
            requested = true;
         }
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         byte boolByte = 0;
         if (processing) {
            boolByte = 1;
         }
         string digit = boolByte.ToString("X2");
         stream.WriteByte((byte)digit[1]);
         boolByte = 0;
         if (requested) {
            boolByte = 1;
         }
         digit = boolByte.ToString("X2");
         stream.WriteByte((byte)digit[1]);
      }
   }
}
