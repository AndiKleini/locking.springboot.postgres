package org.distributed.consensus.model;

public class BookingPolicies {
    public static Boolean canBook(Booking booking) {
        // here are a couple of expensive checks  like:
        // checking if the location was already banned from the platform
        // (e.g.: as a consequence of bad feedback).
        // checking if authorities locked this location
        // (e.g.: risk of life and health)
        // checking if the location was already shut down
        // ...
        return true;
    }
}
