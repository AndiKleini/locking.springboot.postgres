package org.distributed.consensus.controller;

import org.distributed.consensus.model.Booking;
import org.distributed.consensus.model.BookingConfirmation;
import org.distributed.consensus.repository.BookingConfirmationRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
@RequestMapping("/optimistic")
public class OptimisticBookingConfirmationController {

    @Autowired
    BookingConfirmationRepository bookingRepository;

    @GetMapping("/bookingconfirmation/{id}")
    public ResponseEntity<BookingConfirmation> getBookingConfirmations(@PathVariable long id) {
        Optional<BookingConfirmation> confirmation = this.bookingRepository.findById(id);
        return new ResponseEntity<>(confirmation.orElse(null), confirmation.isEmpty() ? HttpStatus.NOT_FOUND : HttpStatus.OK);
    }

    @GetMapping("/bookingconfirmation")
    public ResponseEntity<List<BookingConfirmation>> getBookingConfirmations() {
        List<BookingConfirmation> allBookings = bookingRepository.findAll();
        return new ResponseEntity<>(allBookings, allBookings.isEmpty() ? HttpStatus.NO_CONTENT : HttpStatus.OK);
    }

    @PostMapping("/bookingconfirmation")
    public ResponseEntity<BookingConfirmation> createBooking(@RequestBody BookingConfirmation booking) {
        try {

            // TODO: make sure that the same room cannot be booked more than once
            // intention: as it is expected that a collisions of bookings are rare
            // we believe that we will not run into this error here very often

            BookingConfirmation _bookingConfirmation = bookingRepository.save(new BookingConfirmation(booking.getBookingId()));
            return new ResponseEntity<>(_bookingConfirmation, HttpStatus.CREATED);
        } catch (Exception e) {
            return new ResponseEntity<>(null, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
