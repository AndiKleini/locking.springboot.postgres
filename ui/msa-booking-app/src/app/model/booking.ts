export class Booking {
    public constructor(
        public name: string, 
        public roomId: number, 
        public from: Date, 
        public until: Date) {}
}