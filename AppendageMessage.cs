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

   public class AppendageMessage : UnityMessage {

      public Appendage appendage;

      public int entropicId;

      public AppendageMessage(Appendage appendage, int entropicId) : base(entropicId, 0) {
         this.appendage = appendage;
         messageType = 4;
      }

      public AppendageMessage(string hex) : base(hex) {
         appendage = new Appendage(hex.Substring(20));
         entropicId = chunkX;
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         // legacy chunk position, TODO remove
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)0);
         }
         appendage.Write(stream);
      }
   }
}
