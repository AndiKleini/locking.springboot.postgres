import { Component, OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { Booking } from '../model/booking';
import { RoomService } from '../service/room.service';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [ 
    NgFor
  ],
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css'
})
export class BookingsComponent implements OnInit {
  rooms: Booking[] = [];
  constructor(private roomService: RoomService) {}
  ngOnInit(): void {
    console.log("Before get rooms in comp");
    this.roomService.getRooms().subscribe(r => this.rooms = r);
  }
}