import { Injectable } from '@angular/core';

import { BlogContentBlock, BlogPost, BlogPostQuery, BlogPostResult } from './blog.models';

const sharedContent: BlogContentBlock[] = [
  {
    type: 'paragraph',
    text:
      "Modest fashion is not just about covering up; it is about expressing your personal style with elegance and grace. In this season's collection, we focus on earthy tones and breathable fabrics that provide both comfort and sophistication. Layering is key, allowing you to transition seamlessly from the cool breeze of autumn mornings to the warmer afternoon sun.",
  },
  {
    type: 'heading',
    text: 'Why Earthy Tones?',
  },
  {
    type: 'paragraph',
    text:
      "Nature has always been the ultimate inspiration for designers. This year, we're seeing a resurgence of terracotta, olive green, and deep browns. These colors are versatile and can be mixed and matched effortlessly.",
  },
  {
    type: 'paragraph',
    text:
      'When styling these tones, consider texture. A silk scarf in a rich copper hue adds a touch of luxury to a simple linen dress. The contrast between matte and shiny fabrics creates visual interest without overwhelming the outfit.',
  },
  {
    type: 'product',
    productId: 14,
    productName: 'The Golden Hour Scarf',
    productDescription: 'Crafted from 100% organic silk, perfect for adding a pop of warmth to any ensemble.',
    productImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDAl2zpnIGb9p96VWD1bzsbyYDCKk2RdD-7TiOd0VR41dpaJDFdt4Hx4NoQR0ikKAWotz7zh9vARE_3iZ_Dt88hEA81iS5dZ_v_ZhYmVRNcmA6KvVQxpAy4NwXHyNE9JKAdPDru13UwSdIKycmxm9cSEyvn9s3y--uxJ7HKwQv40QE9suFhq8OlitAGp8bLPvl1TmIh26oWJ2HMTMBwq8bMT64NVrxNKe4svmaj1vVfWHkorOcbKV4_OVhwei5Gotb_zDO6Yv-XE0Q',
    productImageAlt: 'Close up of a mustard yellow hijab',
  },
  {
    type: 'heading',
    text: 'The Art of Layering',
  },
  {
    type: 'paragraph',
    text:
      "Layering isn't just practical; it's an art form. Start with a lightweight base, like our classic cotton abaya, and add structure with a long-line cardigan or a structured blazer.",
  },
  {
    type: 'blockquote',
    text: '"Fashion is the armor to survive the reality of everyday life. Modesty is the shield that protects your peace."',
  },
  {
    type: 'paragraph',
    text:
      'Accessories play a crucial role as well. A thin belt can define the waist without compromising modesty, while statement jewelry can elevate a simple look for an evening event. Remember, the goal is harmony. Each piece should complement the others, creating a cohesive silhouette.',
  },
  {
    type: 'paragraph',
    text:
      "As we move forward into the colder months, don't be afraid to experiment. Fashion is personal. Use these trends as a guide, but always stay true to what makes you feel confident and beautiful.",
  },
];

const posts: BlogPost[] = [
  {
    id: 1,
    slug: 'summer-modesty-stay-cool-and-covered',
    title: 'Summer Modesty: How to Stay Cool and Covered',
    excerpt:
      'Discover the best breathable fabrics and layering techniques to maintain your personal style and comfort during the warmer months without compromising on modesty.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuBNGqRI8mbwuU74Oenw72ga9HiuAdOWjoe7vF_fmh5QaLoT--WI_teRSizdCjbI63MD9uDEQOCRkLIipu2BJ886B6Zzf8EKnQi2N19QQZam5HpwQ-UeRV9cV3mfLiXFPN2KdDp8NKDIdQEFuvkSFe9f-kaK_EmZDQgYqV0C1fjeciXFAn81xmZuOEwoKmEQqRjfelAwAEwiXgEmqNHDZyKJqQQB43kQLw9HXsoj7DbLWUCnzwTyI3NjzQ45CiAJR24rmd_awm8xT4c',
    category: 'Style Guide',
    authorName: 'Amina Khan',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDOeayVSwQjnUOIlrLGb40bJACSOrgMhmcMNpmK5GLQ9_8PTCyQpj6JO-p_BFQwGzsKfydm1khc3mNBngB2EaGx13ARPVkkPtPjGSHyTp9kQhDmD9iBpOjwIMVZ0Yxi0w2WpZCoUH3mP8CrDXHo7mHA9_mCYcslil4Keoho0cJqWk9EVpuuvgJWpG8s6lHciOuuxhkH4oEauV5wUrvjfQhDCak3PoZFHwb4Kjt_i4KQ3aiHygfyjMIOm5kJoGv8krYpvhDcrain4yc',
    authorBio:
      'Fashion enthusiast and content creator at Arza. I love exploring the intersection of modesty and modern trends.',
    publishedAt: new Date('2024-05-15'),
    readTime: '5 min read',
    tags: ['ModestFashion', 'SummerStyle', 'Layering'],
    featured: true,
    coverImageCaption: 'Exploring airy fabrics and breathable layers for summer days.',
    content: sharedContent,
  },
  {
    id: 2,
    slug: 'sustainable-fabrics-changing-modest-fashion',
    title: 'Sustainable Fabrics Changing Modest Fashion',
    excerpt:
      'From Tencel to organic linen, explore how eco-friendly materials are reshaping the industry and your wardrobe choices.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuCoSLuL6Q1Mx7SqAoQSlBSDcTTDWwWLrwMaFjb1mV-9DLVx5tTV4q01CxqcoHLYs69F9w0GBiwJy6v3Ax00hq-QsVB62THLpF85hU3EChyU_wDiMqP0e5UcZdRQK2TP4Ym9rgN4kJJT2fKOqNgg-e_oIOALjl0KATim12CqlehVN38Y54UxiG6yepBD-Za7ueaeEhdNDf4Tj0arhW4pu-oe6k_lFsoWjY0wV6wJcXMBVZFxOP-1Wen8jZ12X5H3UgNGf3ctf4LgT8k',
    category: 'Trends',
    authorName: 'Sarah Ahmed',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDOeayVSwQjnUOIlrLGb40bJACSOrgMhmcMNpmK5GLQ9_8PTCyQpj6JO-p_BFQwGzsKfydm1khc3mNBngB2EaGx13ARPVkkPtPjGSHyTp9kQhDmD9iBpOjwIMVZ0Yxi0w2WpZCoUH3mP8CrDXHo7mHA9_mCYcslil4Keoho0cJqWk9EVpuuvgJWpG8s6lHciOuuxhkH4oEauV5wUrvjfQhDCak3PoZFHwb4Kjt_i4KQ3aiHygfyjMIOm5kJoGv8krYpvhDcrain4yc',
    authorBio: 'Sustainability writer focused on ethical fashion and mindful shopping.',
    publishedAt: new Date('2024-05-12'),
    readTime: '6 min read',
    tags: ['Sustainability', 'EcoFriendly', 'Wardrobe'],
    featured: false,
    coverImageCaption: 'A closer look at textured eco-friendly fabrics.',
    content: sharedContent,
  },
  {
    id: 3,
    slug: 'essential-hijab-styles-modern-workplace',
    title: '5 Essential Hijab Styles for the Modern Workplace',
    excerpt:
      'Look professional and feel confident with these five simple, elegant, and secure hijab wraps designed for the office.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuAMnHp3XwjAOVT5m-aREBcgvLBF2CHpknEpMTI2V3Y4hX8XKGbe70HOQcUQlFeoTcep0c71lzvXPsY0DZe_A8EWvvikfLNd0W6EyfsuJPUIwwM0Y-eHuT5rG-y52_DV8UaGCZhsD4q3r7q5w2MOaKb4RfH7uKfctaviukTu0od2zDPuzQN-IrO3Q2fZcP2UhcbOSrmvXuIAmyHFP42Z2WSieCPdM0wjyPyuxC2Xthaa13BprTsesjj20U75d8GLKyRaxITNBPbE4SY',
    category: 'Style Guide',
    authorName: 'Fatima Khan',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ',
    authorBio: 'Styling lead specializing in professional modest fashion.',
    publishedAt: new Date('2024-05-10'),
    readTime: '4 min read',
    tags: ['Hijab', 'Workwear', 'Style'],
    featured: false,
    coverImageCaption: 'Elegant office-ready hijab styling.',
    content: sharedContent,
  },
  {
    id: 4,
    slug: 'morning-routines-productive-day',
    title: 'Morning Routines for a Productive Day',
    excerpt:
      'Start your day with intention. We break down a Fajr-centered morning routine that boosts productivity and spiritual mindfulness.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDqTd2i-40zbf897VfSkb0a0gX3QrmijuE7XIht3sd2QupwZ-RlDRkMNht4GfGj8LObQkANOnckcvwiUyF9SXRhTrr0fgiYlMDtcU9MKK14929IyuLEJ2J-2kRkxTSMZc_qsmPSr3Y1KWPsMAIvV0hhLyZowQ6HgSuAIzyt2WjAC8GcK1Uek7OkzazJ7HD9tq1bHj0_UaBwPeOwXwozvgqw4tr_SY2lCk-Rgai_OuS1nYF31CsHmi8ex6MQbNPUKcPGK5xGLF0wXvQ',
    category: 'Lifestyle',
    authorName: 'Amina Ross',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDOeayVSwQjnUOIlrLGb40bJACSOrgMhmcMNpmK5GLQ9_8PTCyQpj6JO-p_BFQwGzsKfydm1khc3mNBngB2EaGx13ARPVkkPtPjGSHyTp9kQhDmD9iBpOjwIMVZ0Yxi0w2WpZCoUH3mP8CrDXHo7mHA9_mCYcslil4Keoho0cJqWk9EVpuuvgJWpG8s6lHciOuuxhkH4oEauV5wUrvjfQhDCak3PoZFHwb4Kjt_i4KQ3aiHygfyjMIOm5kJoGv8krYpvhDcrain4yc',
    authorBio: 'Lifestyle editor sharing mindful routines and wellness tips.',
    publishedAt: new Date('2024-05-08'),
    readTime: '5 min read',
    tags: ['Lifestyle', 'Wellness', 'Productivity'],
    featured: false,
    coverImageCaption: 'A peaceful morning ritual with coffee and a good book.',
    content: sharedContent,
  },
  {
    id: 5,
    slug: 'sneak-peek-upcoming-eid-collection',
    title: 'Sneak Peek: The Upcoming Eid Collection',
    excerpt:
      'Get an exclusive first look at our upcoming festive range, featuring rich jewel tones and intricate embroidery.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuBp-E33MKU2ZtFYpexHyaFdAggRbYS3AEKiLrFwcKjnydqsFjgtGHONxq5em9I69mdginhZZYSjViKgALOgMTG7M7_598nNyjg9zhOXjfON-Esvr6WmxRxvFCLW6fg7R6XOSJqt9YV2DMqUo5do6Ifo_KJPUbx7WBZX5Dh4KO3KtSG68K_1pwqbBNvnsfh2p8y4FwUwtJBj9vOSI9U5N3V76hUBDbXW2cqHVDMSTLAOJo_6Cmj0iXzKViIiypwB7SHVHO9k9vd60A4',
    category: 'Collections',
    authorName: 'Team Arza',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4',
    authorBio: 'Our in-house team highlights seasonal collections and launches.',
    publishedAt: new Date('2024-05-05'),
    readTime: '3 min read',
    tags: ['Collections', 'Eid', 'NewArrivals'],
    featured: false,
    coverImageCaption: 'A preview of our jewel-toned Eid collection.',
    content: sharedContent,
  },
  {
    id: 6,
    slug: 'finding-stillness-in-a-busy-world',
    title: 'Finding Stillness in a Busy World',
    excerpt:
      'Reflections on maintaining a spiritual connection amidst the chaos of modern life, work, and social media.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuAieMKud1SR0wT4bOSc-rWnTUBrYj14weDG88yKBHtM8Icnr-ApTnJAgv0wiQ9QQroBBgnb90l3hr8alalKQWbeTU3_pIfOvJbL95or-OrD9MSmnvU_RW7oGszgiFlzRGfyt3UmUeotsQdnl_r8IN4Ux_n0m7SAUnTVZ1OWm611Z4avTpTt5TcIk1KkxRZirj4CBPE2JmBp4XoWRCiTk9mPHEs5b-W_rYcZOcO-SLnfYHfsO_mwY9zkJ1yKFantci6N3-WzqXpCaw8',
    category: 'Faith',
    authorName: 'Zainab Ali',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4',
    authorBio: 'Faith writer focusing on mindful living and spiritual balance.',
    publishedAt: new Date('2024-05-02'),
    readTime: '4 min read',
    tags: ['Faith', 'Mindfulness', 'Spirituality'],
    featured: false,
    coverImageCaption: 'A quiet moment of reflection and prayer.',
    content: sharedContent,
  },
  {
    id: 7,
    slug: 'halal-beauty-brands-to-know',
    title: 'Halal Beauty Brands You Need to Know',
    excerpt:
      'A curated list of certified Halal makeup and skincare brands that deliver both quality and peace of mind.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuCX8A9R99qs3i_ijEz6NF9o7enZIUL582O6Y2RhcESir7-RTHavkEnQBzoE6RWH5z4iCNaDXfSlJcKl2wEaTPSn5Gwzrja8cs6rMjNy0d1x2Ouy7dJVVyXUhh-R5YkPPR1XctpufUo3T45Z7iWvk_6KNjKINvWaipdId6-ErrSwKcYxqXtLH3T_CoReob0f7eBtvKrpm78uttj6s2TWPr8u3kj5hU-Ob3Y2_QNZDkX6p78L2HQ3_z7DiuEOwgGQwjpFuixenjwv7mc',
    category: 'Beauty',
    authorName: 'Layla Omar',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ',
    authorBio: 'Beauty editor spotlighting halal-certified favorites.',
    publishedAt: new Date('2024-04-29'),
    readTime: '3 min read',
    tags: ['Beauty', 'Halal', 'Skincare'],
    featured: false,
    coverImageCaption: 'Minimal makeup essentials for everyday wear.',
    content: sharedContent,
  },
  {
    id: 8,
    slug: 'mastering-minimalist-hijab-drape',
    title: 'Mastering the Minimalist Hijab Drape',
    excerpt: 'A step-by-step guide to a clean, elegant hijab style that works for any occasion.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuAFWjxTIrN9gZyjoaeIdoRsDSvq5PdxE4d-PB16QylnZ4AEq1p2APgbTWRyC-cnmUPX8ssmIoC-U45w5dMmRF9op6peWoEOf5mgwES1WaKLpm3oINF6oyma918cRQd4E3tsbdFWW-0pP2u1LClSxiafwTtEGBv8bE08GpCQAWn29tCcUxsQkX4mWiygcZ2Pghvu_5D8_b_brJVd0_JgroLphsPgVG_bcTGpIrVzjUQyy-5Kwqi4F_d8mSSdRo4odEjFLaMYpNvqmHM',
    category: 'Tutorial',
    authorName: 'Rania Noor',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4',
    authorBio: 'Tutorial creator specializing in easy, elegant hijab styles.',
    publishedAt: new Date('2024-04-20'),
    readTime: '4 min read',
    tags: ['Tutorial', 'Hijab', 'Styling'],
    featured: false,
    coverImageCaption: 'A minimalist hijab drape for everyday wear.',
    content: sharedContent,
  },
  {
    id: 9,
    slug: 'neutral-tones-timeless-wardrobe-staple',
    title: 'Neutral Tones: A Timeless Wardrobe Staple',
    excerpt: 'Why soft neutrals remain the foundation of modest fashion and how to style them year-round.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDJtFfMuJ1xEJhCgT7DYHFctXiZhcWYPrfzgRK6rEOCXkayrMzNClGCgaFq7QPOJBkvKUv3r8ok38In7OAzn8V7M0roHoT1v5cSn20_WSJUa6dMZfPkDp0Df-ZvrcrdQGe0_XMQklJONeobBtR9mUkmFXNyxvSqbzZoGdb2BGquHhJ-yHI8cLj_A0Z0eqSN0DRwLE9M5-6QaIB7Eg1whmi-Qdx0kB4lrU0YCsW_LpqmWACVrJ_ZhnXf1A5IFLqxDSyI9cYeZ29cnT0',
    category: 'Trends',
    authorName: 'Nadia Yusuf',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDOeayVSwQjnUOIlrLGb40bJACSOrgMhmcMNpmK5GLQ9_8PTCyQpj6JO-p_BFQwGzsKfydm1khc3mNBngB2EaGx13ARPVkkPtPjGSHyTp9kQhDmD9iBpOjwIMVZ0Yxi0w2WpZCoUH3mP8CrDXHo7mHA9_mCYcslil4Keoho0cJqWk9EVpuuvgJWpG8s6lHciOuuxhkH4oEauV5wUrvjfQhDCak3PoZFHwb4Kjt_i4KQ3aiHygfyjMIOm5kJoGv8krYpvhDcrain4yc',
    authorBio: 'Trend analyst highlighting timeless wardrobe essentials.',
    publishedAt: new Date('2024-04-12'),
    readTime: '5 min read',
    tags: ['Trends', 'Neutrals', 'Wardrobe'],
    featured: false,
    coverImageCaption: 'Neutral tone layering for modern modest outfits.',
    content: sharedContent,
  },
  {
    id: 10,
    slug: 'why-we-choose-ethical-fabrics',
    title: 'Why We Choose Ethical Fabrics',
    excerpt: 'Behind the scenes of our sourcing process and what ethical fashion means to us.',
    coverImage:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuCYDCPfHt0sXGJYq8fv99Y3oTwy4NhNw3zx5yZi9JET2AqXpUj0qdM0BjfD2DvDe42o11UqV5nhvmdjFK9asKLeMntY2Ipj73xV9PVo06SfawgydLxhtFmbUGJihuHN14j1tx8HEYYkdkAirzmVqQP6RvCm4hHbplylE_jh307RPN_1CpWhBXqLCiqU5mI31oGxszTTaAoTEF9_ePQ-bqdOOH7FStNPNky863QJpeCs4ClVFHbnSErnf1mkwOfymHxzhDkyDbuCJgw',
    category: 'Sustainability',
    authorName: 'Huda Rahman',
    authorAvatar:
      'https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ',
    authorBio: 'Ethical sourcing manager and sustainability advocate.',
    publishedAt: new Date('2024-04-01'),
    readTime: '6 min read',
    tags: ['Sustainability', 'Ethical', 'Fabrics'],
    featured: false,
    coverImageCaption: 'Sustainable fabrics chosen for mindful wardrobes.',
    content: sharedContent,
  },
];

@Injectable({ providedIn: 'root' })
export class BlogService {
  getFeaturedPost(): BlogPost | undefined {
    return posts.find((post) => post.featured) ?? posts[0];
  }

  getPosts({ category, search, page = 1, pageSize = 6 }: BlogPostQuery): BlogPostResult {
    const normalizedSearch = search?.trim().toLowerCase() ?? '';

    const filtered = posts
      .filter((post) => {
        if (category && category !== 'All' && post.category !== category) {
          return false;
        }

        if (!normalizedSearch) {
          return true;
        }

        const tagMatch = post.tags.some((tag) => tag.toLowerCase().includes(normalizedSearch));
        return (
          post.title.toLowerCase().includes(normalizedSearch) ||
          post.excerpt.toLowerCase().includes(normalizedSearch) ||
          post.category.toLowerCase().includes(normalizedSearch) ||
          post.authorName.toLowerCase().includes(normalizedSearch) ||
          tagMatch
        );
      })
      .sort((a, b) => b.publishedAt.getTime() - a.publishedAt.getTime());

    const total = filtered.length;
    const start = (page - 1) * pageSize;
    const paginated = filtered.slice(start, start + pageSize);

    return {
      posts: paginated,
      total,
      page,
      pageSize,
    };
  }

  getPostBySlug(slug: string): BlogPost | undefined {
    return posts.find((post) => post.slug === slug);
  }

  getRelatedPosts(slug: string, limit = 3): BlogPost[] {
    const current = this.getPostBySlug(slug);
    if (!current) {
      return [];
    }

    return posts
      .filter((post) => post.slug !== slug && post.category === current.category)
      .sort((a, b) => b.publishedAt.getTime() - a.publishedAt.getTime())
      .slice(0, limit);
  }
}
