import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TestimonialService } from '../../../../core/services/testimonial.service';
import { Testimonial } from '../../../../core/models/testimonial';

@Component({
  selector: 'app-testimonials',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './testimonials.component.html',
  styleUrl: './testimonials.component.css',
})
export class TestimonialsComponent implements OnInit {
  testimonials: Testimonial[] = [];
  stars = [1, 2, 3, 4, 5];

  constructor(private readonly testimonialService: TestimonialService) {}

  ngOnInit(): void {
    this.testimonialService.getTestimonials().subscribe((testimonials) => {
      this.testimonials = testimonials;
    });
  }

  getStarIcon(rating: number, star: number): string {
    if (rating >= star) {
      return 'star';
    }

    if (rating + 0.5 >= star) {
      return 'star_half';
    }

    return 'star_outline';
  }
}
