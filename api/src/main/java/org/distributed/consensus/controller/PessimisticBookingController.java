package org.distributed.consensus.controller;

import org.distributed.consensus.model.Booking;
import org.distributed.consensus.model.BookingPolicies;
import org.distributed.consensus.repository.BookingRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/pessimistic")
public class PessimisticBookingController {

    @Autowired
    BookingRepository bookingRepository;

    @PostMapping("/booking")
    public ResponseEntity<Booking> createBooking(@RequestBody Booking booking) {

        // unfortunately we cannot guarantee that all locations which might
        // evaluate to false here are not somehow offered to our customers
        // (e.g.: shared links in social medias, clients that are working
        // with old data ... )
        // Therefore this check is crucial
        if (!BookingPolicies.canBook(booking)) {
            return new ResponseEntity<>(null, HttpStatus.CONFLICT);
        }

        // TODO: assure that the same room cannot be booked twice
        // intention: it might happen for some locations that booking attempts to the same rooms are
        // made in parallel. Consequently it is better when we prevent this very early in the process

        try {
            Booking _booking = bookingRepository.save(new Booking(booking.getName(), booking.getRoomId(), booking.getStart(), booking.getFinish()));
            return new ResponseEntity<>(_booking, HttpStatus.CREATED);
        } catch (Exception e) {
            return new ResponseEntity<>(null, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @DeleteMapping("/booking/{id}")
    public ResponseEntity<HttpStatus> deleteBooking(@PathVariable("id") long id) {
        try {
            bookingRepository.deleteById(id);
            return new ResponseEntity<>(HttpStatus.NO_CONTENT);
        } catch (Exception e) {
            return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
