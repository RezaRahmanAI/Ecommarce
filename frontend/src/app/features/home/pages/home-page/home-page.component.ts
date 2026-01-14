import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HeroComponent } from '../../components/hero/hero.component';
import { CategoryGridComponent } from '../../components/category-grid/category-grid.component';
import { NewArrivalsComponent } from '../../components/new-arrivals/new-arrivals.component';
import { PromoBannerComponent } from '../../components/promo-banner/promo-banner.component';
import { FeaturedProductsComponent } from '../../components/featured-products/featured-products.component';
import { WhyChooseUsComponent } from '../../components/why-choose-us/why-choose-us.component';
import { TestimonialsComponent } from '../../components/testimonials/testimonials.component';
import { NewsletterComponent } from '../../components/newsletter/newsletter.component';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    CommonModule,
    HeroComponent,
    CategoryGridComponent,
    NewArrivalsComponent,
    PromoBannerComponent,
    FeaturedProductsComponent,
    WhyChooseUsComponent,
    TestimonialsComponent,
    NewsletterComponent,
  ],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
})
export class HomePageComponent {}
