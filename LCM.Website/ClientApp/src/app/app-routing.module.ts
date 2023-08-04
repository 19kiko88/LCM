import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BsEighteenComponent } from './home/pages/bs-eighteen/bs-eighteen.component';
import { ManualCloseComponent } from './home/pages/manual-close/manual-close.component';
import { TabBsComponent } from './home/pages/tab-bs/tab-bs.component';

const routes: Routes = [
  {path: '', component: BsEighteenComponent },
  {path: 'tab', component: TabBsComponent },//使用mat-tab-group頁籤
  {path: 'bs18', component: BsEighteenComponent},
  {path: 'manual_close', component: ManualCloseComponent },  
  {path: '**', redirectTo: 'bs18' }//沒有比對到路由
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
