import { Booking } from "../model/booking";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root'})
export class RoomService {
    constructor(private http: HttpClient) {}
    getRooms() : Observable<Booking[]> { 
        console.log("Before called bookings");
        return this.http.get<Booking[]>("http://localhost:8080/optimistic/booking");
    }
}