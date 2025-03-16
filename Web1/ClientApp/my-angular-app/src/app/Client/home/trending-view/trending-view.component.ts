import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Tintuc } from '../../models/tintuc';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-trending-view',
  templateUrl: './trending-view.component.html',
  styleUrl: './trending-view.component.css'
})
export class TrendingViewComponent implements OnInit {

    private aws_URL = environment.awsUrl;
    @Input() tintucs: Tintuc[] = [];

    tintucTop: Tintuc[] = [];

    constructor() {}

    ngOnInit(): void {
      console.log("Component con:" ,this.tintucs);
      this.getTopNews();
    }

    getFullImageUrl(imageUrl: string): string {
      return `${this.aws_URL}/${imageUrl}`;
    }

    getTopNews() {
      this.tintucTop = this.tintucs
        .sort((a, b) => b.luongKhachTruyCap - a.luongKhachTruyCap) 
        .slice(0, 4); 
    }

    Ghi() {
      console.log("OK");
    }
}
