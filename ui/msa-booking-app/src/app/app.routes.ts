import { Routes } from '@angular/router';
import { BookingsComponent } from './bookings/bookings.component';

export const routes: Routes = [
    { path: 'bookings', component: BookingsComponent },
    { path: '', component: BookingsComponent }
];