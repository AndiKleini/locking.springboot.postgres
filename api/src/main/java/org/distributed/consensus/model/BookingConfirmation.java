package org.distributed.consensus.model;

import jakarta.persistence.*;

@Entity
@Table(name = "bookingsconfirmation")
public class BookingConfirmation {

    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    private long id;

    @Column(name = "bookinId")
    private long bookingId;

    public BookingConfirmation(long bookingId) {
        this.bookingId = bookingId;
    }

    public BookingConfirmation() {}

    public long getBookingId() {
        return bookingId;
    }
}
