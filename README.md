# Median-Priority-Queue-Using-CSharp
This project introduces a novel data structure, akin to a priority queue, optimized for rapid retrieval of maximum, minimum, and median values. The structure is built around four heaps—two min heaps and two max heaps—offering O(1) time complexity for operations such as Max, Min, and Median. Additional operations like DeleteMax(), DeleteMin(), and Insert() showcase logarithmic time complexity O(log n).

The data is distributed across two primary heaps: a "half small max heap" housing half of the data with the smallest priority, and a "half big max heap" containing half of the data with the highest priority. Correspondingly, there are two secondary heaps mirroring the primary ones, but with min priority order.
