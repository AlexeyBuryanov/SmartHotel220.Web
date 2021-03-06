﻿import * as React from 'react';
import { Footer } from './Footer';

// Общая разметка
export default class Layout extends React.Component<{}, {}> {

    public render() {
        return <div className='sh-site'>
                   <section className='sh-content'>
                       {this.props.children}
                   </section>
                   <Footer/>
               </div>;
    }
}
